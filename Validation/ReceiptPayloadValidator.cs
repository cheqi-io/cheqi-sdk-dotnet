using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;
using Wasmtime;

namespace Cheqi.Sdk.Validation;

/// <summary>Runs the same pinned, offline receipt validator as the Java and Python SDKs.</summary>
public sealed class ReceiptPayloadValidator
{
    private const int MaxInputBytes = 2 * 1024 * 1024;
    private static readonly Lazy<(Engine Engine, Module Module)> Runtime = new(LoadRuntime);
    private static readonly JsonSerializerOptions ReportJson = new(JsonSerializerDefaults.Web);
    private static readonly JsonSerializerOptions PayloadJson = CreatePayloadOptions();

    /// <summary>Reports all findings without changing the supplied receipt or contacting Cheqi.</summary>
    public InvoiceValidationReport Validate(ReceiptPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        try { return ValidateJson(JsonSerializer.Serialize(payload, PayloadJson)); }
        catch (CheqiSdkException) { throw; }
        catch (Exception exception) when (exception is JsonException or ArgumentException or InvalidOperationException)
        {
            throw new CheqiSdkException("Receipt payload cannot be serialized", exception,
                CheqiSdkException.ErrorCodes.VALIDATION_ERROR, 0, null);
        }
    }

    /// <summary>Validates public receipt JSON, including malformed input, against the shared rule set.</summary>
    public InvoiceValidationReport ValidateJson(string payloadJson)
    {
        ArgumentNullException.ThrowIfNull(payloadJson);
        try
        {
            var (engine, module) = Runtime.Value;
            using var store = new Store(engine);
            store.SetLimits(memorySize: 64 * 1024 * 1024, instances: 1, memories: 1);
            store.Fuel = 200_000_000;
            using var linker = new Linker(engine);
            var instance = linker.Instantiate(store, module);
            var memory = instance.GetMemory("memory") ?? throw new InvalidDataException("Missing validator memory");
            int Call(string name) => (instance.GetFunction<int>("cheqi_validator_" + name)
                ?? throw new InvalidDataException("Missing validator export: " + name))();
            if (Call("abi_version") != 1 || memory.Maximum != 1024)
                throw new InvalidDataException("Unsupported validator ABI or memory limit");

            int byteCount = Encoding.UTF8.GetByteCount(payloadJson);
            var allocate = instance.GetFunction<int, int>("cheqi_validator_alloc")
                ?? throw new InvalidDataException("Missing validator allocator");
            int pointer = allocate(Math.Min(byteCount, MaxInputBytes + 1));
            int length;
            if (pointer != 0)
            {
                if (byteCount > MaxInputBytes) throw new InvalidDataException("Validator accepted oversized input");
                Encoding.UTF8.GetBytes(payloadJson, memory.GetSpan(pointer, byteCount));
                length = Call("validate");
            }
            else length = Call("result_len");
            int start = Call("result_ptr");
            if (length <= 0 || length != Call("result_len") || start < 0 || (long)start + length > memory.GetLength())
                throw new InvalidDataException("Invalid validator result bounds");
            var report = JsonSerializer.Deserialize<InvoiceValidationReport>(memory.GetSpan(start, length), ReportJson)
                ?? throw new InvalidDataException("Missing validation report");
            if (report.Profile != "EN16931_CORE_2017_V1" || report.CompleteDocument || report.Issues == null
                || report.Valid != (report.Issues.Count == 0)
                || report.Issues.Any(issue => issue == null || string.IsNullOrWhiteSpace(issue.RuleId)
                    || issue.Path == null || string.IsNullOrWhiteSpace(issue.Message)))
                throw new InvalidDataException("Invalid validation report");
            return report;
        }
        catch (Exception exception)
        {
            throw new CheqiSdkException("Receipt payload validator is unavailable", exception,
                CheqiSdkException.ErrorCodes.CONFIGURATION_ERROR, 0, null);
        }
    }

    /// <summary>Stops issuance with all detected issues before matching, encryption, or upload.</summary>
    public void ValidateOrThrow(ReceiptPayload payload)
    {
        var report = Validate(payload);
        if (!report.Valid)
            throw new CheqiSdkException("Invalid receipt payload: " + string.Join("; ",
                report.Issues.Select(issue => $"{issue.RuleId} — {issue.Path}: {issue.Message}")),
                CheqiSdkException.ErrorCodes.VALIDATION_ERROR, 0, null) { ValidationReport = report };
    }

    private static (Engine, Module) LoadRuntime()
    {
        using var wasm = Resource("cheqi-receipt-validator.wasm");
        using var bytes = new MemoryStream();
        wasm.CopyTo(bytes);
        using var manifest = Resource("manifest.json");
        using var metadata = JsonDocument.Parse(manifest);
        VerifyBundle(bytes.ToArray(), metadata.RootElement);
        using var config = new Wasmtime.Config().WithFuelConsumption(true);
        var engine = new Engine(config);
        Module? module = null;
        try
        {
            module = Module.FromBytes(engine, "cheqi-receipt-validator", bytes.ToArray());
            if (module.Imports.Count != 0) throw new InvalidDataException("Validator must not require host imports");
            return (engine, module);
        }
        catch { module?.Dispose(); engine.Dispose(); throw; }
    }

    internal static void VerifyBundle(byte[] wasm, JsonElement manifest)
    {
        if (manifest.GetProperty("abiVersion").GetInt32() != 1
            || manifest.GetProperty("sourceDirty").GetBoolean()
            || manifest.GetProperty("target").GetString() != "wasm32-unknown-unknown"
            || !Regex.IsMatch(manifest.GetProperty("sourceCommit").GetString() ?? "", "\\A[0-9a-f]{40}\\z")
            || string.IsNullOrWhiteSpace(manifest.GetProperty("engineVersion").GetString()))
            throw new InvalidDataException("Invalid validator provenance or ABI");
        if (!string.Equals(Convert.ToHexString(SHA256.HashData(wasm)), manifest.GetProperty("sha256").GetString(),
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Validator checksum mismatch");
    }

    private static Stream Resource(string name) => typeof(ReceiptPayloadValidator).Assembly
        .GetManifestResourceStream("Cheqi.Sdk.Validation.Resources." + name)
        ?? throw new FileNotFoundException("Missing bundled validator resource", name);

    private static JsonSerializerOptions CreatePayloadOptions()
    {
        var options = CheqiJson.CreateOptions();
        // The shared validator accepts decimal strings, preserving every supplied digit (as Java does).
        options.Converters.Insert(0, new ExactDecimalConverter());
        return options;
    }

    private sealed class ExactDecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) => reader.GetDecimal();
        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }
}
