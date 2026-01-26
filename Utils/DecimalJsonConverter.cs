using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Utils;

/// <summary>
/// Custom JSON converter for decimal that strips trailing zeros.
/// This matches the behavior of the Java SDK's ObjectMapperConfig.
/// </summary>
public class DecimalJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }
        
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (decimal.TryParse(stringValue, out var result))
            {
                return result;
            }
        }
        
        throw new JsonException($"Unable to convert {reader.TokenType} to decimal");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        // Strip trailing zeros by converting to string and removing them
        // Use InvariantCulture to ensure JSON-compatible output (dot as decimal separator, no thousand separators)
        var stringValue = value.ToString("G29", CultureInfo.InvariantCulture); // G29 uses general format with max precision
        
        // Remove trailing zeros after decimal point
        if (stringValue.Contains('.'))
        {
            stringValue = stringValue.TrimEnd('0').TrimEnd('.');
        }
        
        // Write as raw number (not quoted string)
        writer.WriteRawValue(stringValue);
    }
}

/// <summary>
/// Custom JSON converter for nullable decimal that strips trailing zeros.
/// </summary>
public class NullableDecimalJsonConverter : JsonConverter<decimal?>
{
    private readonly DecimalJsonConverter _innerConverter = new();

    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }
        
        return _innerConverter.Read(ref reader, typeof(decimal), options);
    }

    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            _innerConverter.Write(writer, value.Value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
