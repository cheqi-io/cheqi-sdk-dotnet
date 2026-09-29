using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Serialization;

internal sealed class DoubleJsonConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetDouble();
    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        => writer.WriteRawValue(CheqiNumber.PlainDouble(value, retainFloatingScale: false));
}
