using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// JSON converter for UnitCode that serializes/deserializes using UN/ECE codes from EnumMember attributes.
/// </summary>
public class UnitCodeJsonConverter : JsonConverter<UnitCode>
{
    public override UnitCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var code = reader.GetString();
        if (code == null)
            throw new JsonException("Unit code cannot be null");

        foreach (UnitCode unitCode in Enum.GetValues(typeof(UnitCode)))
        {
            var memberInfo = typeof(UnitCode).GetField(unitCode.ToString());
            var enumMemberAttr = memberInfo?.GetCustomAttribute<EnumMemberAttribute>();
            
            if (enumMemberAttr?.Value?.Equals(code, StringComparison.OrdinalIgnoreCase) == true)
            {
                return unitCode;
            }
        }

        throw new JsonException($"Unknown unit code: {code}. See UN/ECE Recommendation 20 for valid codes.");
    }

    public override void Write(Utf8JsonWriter writer, UnitCode value, JsonSerializerOptions options)
    {
        var memberInfo = typeof(UnitCode).GetField(value.ToString());
        var enumMemberAttr = memberInfo?.GetCustomAttribute<EnumMemberAttribute>();
        
        writer.WriteStringValue(enumMemberAttr?.Value ?? value.ToString());
    }
}
