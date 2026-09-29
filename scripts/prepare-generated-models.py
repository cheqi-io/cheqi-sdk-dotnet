"""Normalize model-only generator output and maintain explicit converter registration."""
from pathlib import Path
import re

root = Path(__file__).resolve().parent.parent
models = root / "Models/Generated"
for path in models.glob("*.cs"):
    source = path.read_text().replace("using Cheqi.Sdk.Client;", "").replace("namespace Cheqi.Sdk.Client", "namespace Cheqi.Sdk.Models.Generated")
    source = source.replace("JsonSerializer.Deserialize<DateTime>", "JsonSerializer.Deserialize<DateTimeOffset>")
    # The generator's oneOf converter requires a full generated HTTP client and
    # cannot discriminate optional-property schemas. Discriminate by contract fields.
    if path.name == "SubmitEncryptedReceipt202Response.cs" and "ClientUtils.TryDeserialize" in source:
        begin = source.index("            int currentDepth =", source.index("public override SubmitEncryptedReceipt202Response Read"))
        end = source.index("\n        }", begin)
        source = source[:begin] + '''            using var document = JsonDocument.ParseValue(ref utf8JsonReader);
            var root = document.RootElement;
            if (root.TryGetProperty("state", out _))
                return new SubmitEncryptedReceipt202Response(root.Deserialize<MatchStatusResponse>(jsonSerializerOptions)!);
            if (root.TryGetProperty("cheqiReceiptId", out _))
                return new SubmitEncryptedReceipt202Response(root.Deserialize<ReceiptSubmissionResponse>(jsonSerializerOptions)!);
            throw new JsonException("Expected a submission or match status response");''' + source[end:]
        begin = source.index("            writer.WriteStartObject();", source.index("public override void Write"))
        end = source.index("\n        }", begin)
        source = source[:begin] + '''            if (submitEncryptedReceipt202Response.ReceiptSubmissionResponse != null)
                JsonSerializer.Serialize(writer, submitEncryptedReceipt202Response.ReceiptSubmissionResponse, jsonSerializerOptions);
            else if (submitEncryptedReceipt202Response.MatchStatusResponse != null)
                JsonSerializer.Serialize(writer, submitEncryptedReceipt202Response.MatchStatusResponse, jsonSerializerOptions);
            else throw new JsonException("Expected a submission or match status response");''' + source[end:]
    # Optional enum properties must stay absent when not supplied (Java NON_NULL).
    source = re.sub(r'(?m)^(            )(var (\w+)RawValue = [^\n]+\.(\w+)Option\.Value!\.Value[^\n]+;\n            writer.WriteString\([^\n]+;)',
                    lambda m: m[1] + 'if (' + re.search(r'(\w+)\.' + m[4] + 'Option', m[2])[1] + '.' + m[4] + 'Option.IsSet)\n            {\n    ' + m[2].replace('\n', '\n    ') + '\n            }', source)
    if "[Obsolete]" in source and "#pragma warning disable CS0612" not in source:
        source = source.replace("#nullable enable", "#nullable enable\n#pragma warning disable CS0612 // Generated code references its own deprecated wire properties.")
    source = re.sub(r'(?m)^([ \t]*)writer.WriteNumber\("([^"\n]+)", ([^;\n]+)\);',
                    lambda m: m[1] + '{\n' + m[1] + '    writer.WritePropertyName("' + m[2] + '");\n' + m[1]
                    + '    JsonSerializer.Serialize(writer, ' + m[3] + ', jsonSerializerOptions);\n' + m[1] + '}', source)
    source = re.sub(r'(?m)^([ \t]*)writer.WriteString\("([^"\n]+)", ([^;\n]+)\.ToString\(\w+Format\)\);',
                    lambda m: m[1] + '{\n' + m[1] + '    writer.WritePropertyName("' + m[2] + '");\n' + m[1]
                    + '    JsonSerializer.Serialize(writer, ' + m[3] + ', jsonSerializerOptions);\n' + m[1] + '}', source)
    path.write_text(source)
converters = []
for path in sorted(models.glob("*.cs")):
    converters.extend(re.findall(r"public class (\w+) : JsonConverter<", path.read_text()))
output = root / "Serialization/CheqiJson.cs"
source = output.read_text()
begin = source.index("        options.Converters.Add(")
end = source.index("        options.Converters.Add(new Utils.DecimalJsonConverter());", begin)
output.write_text(source[:begin] + ''.join(f"        options.Converters.Add(new {name}());\n" for name in converters) + source[end:])
