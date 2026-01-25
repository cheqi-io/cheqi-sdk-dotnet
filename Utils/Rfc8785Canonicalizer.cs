using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Cheqi.Sdk.Utils;

/// <summary>
/// RFC 8785 compliant JSON canonicalization implementation.
/// 
/// Provides deterministic JSON serialization for consistent hashing and signing.
/// This implementation follows the RFC 8785 specification which defines:
/// - Lexicographic ordering of object keys
/// - Specific string escaping rules
/// - Consistent number representation
/// - No insignificant whitespace
/// 
/// Thread-safe and can be reused across multiple canonicalization operations.
/// </summary>
public class RFC8785Canonicalizer
{
    private readonly ILogger<RFC8785Canonicalizer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new RFC8785Canonicalizer instance.
    /// </summary>
    public RFC8785Canonicalizer(ILogger<RFC8785Canonicalizer>? logger = null)
    {
        _logger = logger ?? NullLogger<RFC8785Canonicalizer>.Instance;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _logger.LogDebug("RFC8785Canonicalizer initialized");
    }

    /// <summary>
    /// Canonicalizes JSON string according to RFC 8785.
    /// </summary>
    /// <param name="json">JSON string to canonicalize</param>
    /// <returns>Canonicalized JSON string</returns>
    /// <exception cref="ArgumentException">if json is null or empty</exception>
    /// <exception cref="JsonException">if JSON parsing fails</exception>
    public string Canonicalize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON string cannot be null or empty", nameof(json));
        }

        _logger.LogDebug("Canonicalizing JSON string");
        var node = JsonNode.Parse(json);
        if (node == null)
        {
            throw new ArgumentException("Invalid JSON", nameof(json));
        }

        string canonical = CanonicalizeNode(node);
        _logger.LogDebug("JSON canonicalization complete");
        return canonical;
    }

    /// <summary>
    /// Canonicalizes an object directly by first converting to JSON.
    /// </summary>
    /// <param name="obj">Object to canonicalize</param>
    /// <returns>Canonicalized JSON string</returns>
    /// <exception cref="ArgumentNullException">if object is null</exception>
    public string Canonicalize(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "Object cannot be null");
        }

        _logger.LogDebug("Canonicalizing object of type: {Type}", obj.GetType().Name);
        string json = JsonSerializer.Serialize(obj, _jsonOptions);
        return Canonicalize(json);
    }

    /// <summary>
    /// Calculates SHA-256 hash of canonicalized JSON.
    /// </summary>
    /// <param name="json">JSON string to hash</param>
    /// <returns>Hex-encoded SHA-256 hash</returns>
    public string CalculateHash(string json)
    {
        _logger.LogDebug("Calculating hash for JSON string");
        string canonical = Canonicalize(json);
        string hash = CalculateSHA256(canonical);
        _logger.LogDebug("Hash calculated: {HashPrefix}...", hash.Substring(0, 8));
        return hash;
    }

    /// <summary>
    /// Calculates SHA-256 hash of canonicalized object.
    /// </summary>
    /// <param name="obj">Object to hash</param>
    /// <returns>Hex-encoded SHA-256 hash</returns>
    public string CalculateHash(object obj)
    {
        _logger.LogDebug("Calculating hash for object of type: {Type}", obj.GetType().Name);
        string canonical = Canonicalize(obj);
        string hash = CalculateSHA256(canonical);
        _logger.LogDebug("Hash calculated: {HashPrefix}...", hash.Substring(0, 8));
        return hash;
    }

    private string CanonicalizeNode(JsonNode? node)
    {
        if (node == null)
            return "null";

        var sb = new StringBuilder();
        WriteCanonical(node, sb);
        return sb.ToString();
    }

    private void WriteCanonical(JsonNode? node, StringBuilder sb)
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

    private void WriteValue(JsonValue value, StringBuilder sb)
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

    private void WriteString(string value, StringBuilder sb)
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

    private void WriteNumber(JsonElement element, StringBuilder sb)
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

    private void WriteArray(JsonArray array, StringBuilder sb)
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

    private void WriteObject(JsonObject obj, StringBuilder sb)
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

    /// <summary>
    /// Calculates SHA-256 hash of the input string
    /// </summary>
    private string CalculateSHA256(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(inputBytes);

        var hexString = new StringBuilder();
        foreach (byte b in hash)
        {
            hexString.Append($"{b:x2}");
        }

        return hexString.ToString();
    }
}
