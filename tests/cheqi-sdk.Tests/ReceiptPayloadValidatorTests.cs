using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;
using Cheqi.Sdk.Tests.TestSupport;
using Cheqi.Sdk.Validation;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class ReceiptPayloadValidatorTests
{
    private static string Fixture(string name) => Path.Combine(AppContext.BaseDirectory, "Fixtures", "Validation", name);
    private static ReceiptPayload Payload() => SdkHarness.Read<ReceiptPayload>(File.ReadAllText(Fixture("receipt-payload.json")));

    public static IEnumerable<object[]> ReferenceCases()
    {
        using var file = File.OpenRead(Fixture("parity-cases.json.gz"));
        using var gzip = new GZipStream(file, CompressionMode.Decompress);
        using var cases = JsonDocument.Parse(gzip);
        foreach (var item in cases.RootElement.EnumerateArray())
            yield return new object[] { item.GetProperty("name").GetString()!, item.GetProperty("input").GetString()!, item.GetProperty("expected").GetString()! };
    }

    [Theory]
    [MemberData(nameof(ReferenceCases))]
    public void MatchesEveryJavaAndPythonReferenceReport(string name, string input, string expected)
    {
        var report = new ReceiptPayloadValidator().ValidateJson(input);
        var actual = JsonSerializer.SerializeToNode(report, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.True(JsonNode.DeepEquals(JsonNode.Parse(expected), actual), $"Report mismatch: {name}\n{actual}");
    }

    [Fact]
    public void ValidationPreservesSuppliedValuesAndNumericWireJson()
    {
        var receipt = Payload();
        var original = JsonSerializer.Serialize(receipt, CheqiJson.CreateOptions());
        var validator = new ReceiptPayloadValidator();
        Assert.True(validator.Validate(receipt).Valid);
        validator.ValidateOrThrow(receipt);
        Assert.Equal(original, JsonSerializer.Serialize(receipt, CheqiJson.CreateOptions()));
        using var json = JsonDocument.Parse(original);
        Assert.Equal(19.41m, json.RootElement.GetProperty("totalAmount").GetDecimal());
        Assert.Equal(8.905m, json.RootElement.GetProperty("products")[0].GetProperty("unitPrice").GetDecimal());
    }

    [Fact]
    public void AllFindingsAreAvailableOnTheSdkException()
    {
        var receipt = Payload();
        receipt.PaymentDetails = new PaymentDetails();
        receipt.Products[0].Taxes = new() { new Tax(9, "VAT", 17.81m) { Amount = 1.6m } };
        var error = Assert.Throws<CheqiSdkException>(() => new ReceiptPayloadValidator().ValidateOrThrow(receipt));
        Assert.True(error.IsValidationError());
        var report = Assert.IsType<InvoiceValidationReport>(error.ValidationReport);
        Assert.False(report.Valid);
        Assert.False(report.CompleteDocument);
        var paths = report.Issues.Select(issue => issue.Path).ToArray();
        Assert.Contains("paymentDetails.amountDue", paths);
        Assert.Contains("paymentDetails.roundingAmount", paths);
        Assert.Contains("products[0].taxes[0].category", paths);
        Assert.All(report.Issues, issue => { Assert.Contains(issue.RuleId, error.Message); Assert.Contains(issue.Path, error.Message); });
    }

    [Fact]
    public async Task ConcurrentCallsHaveIndependentState()
    {
        var validator = new ReceiptPayloadValidator();
        var raw = File.ReadAllText(Fixture("receipt-payload.json"));
        var reports = await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => Task.Run(() => validator.ValidateJson(raw))));
        Assert.All(reports, report => Assert.True(report.Valid));
    }

    [Fact]
    public void CorruptBundleAndDirtyProvenanceAreRejectedBeforeCompilation()
    {
        var manifest = JsonSerializer.SerializeToElement(new { abiVersion = 1, sourceDirty = false,
            target = "wasm32-unknown-unknown", sourceCommit = new string('a', 40), engineVersion = "0.4.4",
            sha256 = Convert.ToHexString(SHA256.HashData("original"u8)) });
        Assert.Contains("checksum mismatch", Assert.Throws<InvalidDataException>(() =>
            ReceiptPayloadValidator.VerifyBundle("corrupted"u8.ToArray(), manifest)).Message);
        var dirty = JsonNode.Parse(manifest.GetRawText())!;
        dirty["sourceDirty"] = true;
        Assert.Throws<InvalidDataException>(() => ReceiptPayloadValidator.VerifyBundle("original"u8.ToArray(),
            JsonSerializer.SerializeToElement(dirty)));
    }

    [Theory]
    [InlineData("issue")]
    [InlineData("resume")]
    [InlineData("download")]
    [InlineData("envelope")]
    public async Task InvalidAmountsStopEveryPayloadEntryPointBeforeHttpOrEncryption(string operation)
    {
        using var harness = new SdkHarness();
        var receipt = SdkHarness.Payload();
        receipt.TotalAmount = 999m;
        var service = harness.Sdk.ReceiptService;
        Assert.False(service.Validate(receipt).Valid);
        var error = await Assert.ThrowsAsync<CheqiSdkException>(async () =>
        {
            switch (operation)
            {
                case "issue": await service.IssueReceiptAsync(SdkHarness.Identification(), receipt); break;
                case "resume": await service.ResumeReceiptAsync("match-original", receipt); break;
                case "download": await service.IssueDownloadReceiptAsync(SdkHarness.Identification(), receipt); break;
                default: service.GenerateEncryptedReceiptEnvelope(SdkHarness.Read<RecipientResolutionResponse>(SdkHarness.Match()), receipt); break;
            }
        });
        Assert.True(error.IsValidationError());
        Assert.NotNull(error.ValidationReport);
        Assert.Empty(harness.Requests);
    }
}
