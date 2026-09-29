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
            if (decimal.TryParse(stringValue, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }
        }
        
        throw new JsonException($"Unable to convert {reader.TokenType} to decimal");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        // Decimal's fixed-point form preserves money exactly and avoids trimming exponent digits.
        writer.WriteRawValue(value.ToString("0.############################", CultureInfo.InvariantCulture));
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
