using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cheqi.Sdk.Utils;

/// <summary>
/// JSON canonicalization per RFC 8785 (JCS - JSON Canonicalization Scheme).
/// </summary>
public static class Rfc8785Canonicalizer
{
    /// <summary>
    /// Canonicalizes a JSON string according to RFC 8785.
    /// </summary>
    /// <param name="json">The JSON string to canonicalize.</param>
    /// <returns>The canonicalized JSON string.</returns>
    public static string Canonicalize(string json)
    {
        if (string.IsNullOrEmpty(json))
            throw new ArgumentException("JSON cannot be null or empty", nameof(json));

        var node = JsonNode.Parse(json);
        if (node == null)
            throw new ArgumentException("Invalid JSON", nameof(json));

        return Canonicalize(node);
    }

    /// <summary>
    /// Canonicalizes a JsonNode according to RFC 8785.
    /// </summary>
    /// <param name="node">The JSON node to canonicalize.</param>
    /// <returns>The canonicalized JSON string.</returns>
    public static string Canonicalize(JsonNode? node)
    {
        if (node == null)
            return "null";

        var sb = new StringBuilder();
        WriteCanonical(node, sb);
        return sb.ToString();
    }

    private static void WriteCanonical(JsonNode? node, StringBuilder sb)
    {
        switch (node)
        {
            case null:
                sb.Append("null");
                break;

            case JsonValue value:
                WriteValue(value, sb);
                break;

            case JsonArray array:
                WriteArray(array, sb);
                break;

            case JsonObject obj:
                WriteObject(obj, sb);
                break;
        }
    }

    private static void WriteValue(JsonValue value, StringBuilder sb)
    {
        var element = value.GetValue<JsonElement>();

        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                WriteString(element.GetString() ?? "", sb);
                break;

            case JsonValueKind.Number:
                WriteNumber(element, sb);
                break;

            case JsonValueKind.True:
                sb.Append("true");
                break;

            case JsonValueKind.False:
                sb.Append("false");
                break;

            case JsonValueKind.Null:
                sb.Append("null");
                break;

            default:
                sb.Append(element.GetRawText());
                break;
        }
    }

    private static void WriteString(string value, StringBuilder sb)
    {
        sb.Append('"');
        foreach (var c in value)
        {
            switch (c)
            {
                case '"':
                    sb.Append("\\\"");
                    break;
                case '\\':
                    sb.Append("\\\\");
                    break;
                case '\b':
                    sb.Append("\\b");
                    break;
                case '\f':
                    sb.Append("\\f");
                    break;
                case '\n':
                    sb.Append("\\n");
                    break;
                case '\r':
                    sb.Append("\\r");
                    break;
                case '\t':
                    sb.Append("\\t");
                    break;
                default:
                    if (c < 0x20)
                    {
                        sb.Append($"\\u{(int)c:x4}");
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    break;
            }
        }
        sb.Append('"');
    }

    private static void WriteNumber(JsonElement element, StringBuilder sb)
    {
        // RFC 8785 requires specific number formatting
        if (element.TryGetInt64(out var longValue))
        {
            sb.Append(longValue);
        }
        else if (element.TryGetDouble(out var doubleValue))
        {
            // Use G17 format for full precision, then clean up
            var formatted = doubleValue.ToString("G17", System.Globalization.CultureInfo.InvariantCulture);

            // Ensure no trailing zeros after decimal point (except required by RFC)
            if (formatted.Contains('.') && !formatted.Contains('E') && !formatted.Contains('e'))
            {
                formatted = formatted.TrimEnd('0').TrimEnd('.');
                if (!formatted.Contains('.'))
                {
                    // Ensure at least one decimal for non-integers that had decimals
                    if (doubleValue != Math.Floor(doubleValue))
                    {
                        formatted += ".0";
                    }
                }
            }

            sb.Append(formatted);
        }
        else
        {
            sb.Append(element.GetRawText());
        }
    }

    private static void WriteArray(JsonArray array, StringBuilder sb)
    {
        sb.Append('[');
        var first = true;
        foreach (var item in array)
        {
            if (!first) sb.Append(',');
            first = false;
            WriteCanonical(item, sb);
        }
        sb.Append(']');
    }

    private static void WriteObject(JsonObject obj, StringBuilder sb)
    {
        sb.Append('{');

        // Sort properties by UTF-16 code unit values (as strings)
        var sortedProperties = obj
            .OrderBy(p => p.Key, StringComparer.Ordinal)
            .ToList();

        var first = true;
        foreach (var (key, value) in sortedProperties)
        {
            if (!first) sb.Append(',');
            first = false;
            WriteString(key, sb);
            sb.Append(':');
            WriteCanonical(value, sb);
        }

        sb.Append('}');
    }
}
