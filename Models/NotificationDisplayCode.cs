using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

public sealed class NotificationDisplayCode
{
    [JsonPropertyName("type")]
    public BarcodeType Type { get; }

    [JsonPropertyName("data")]
    public string Data { get; }

    public NotificationDisplayCode(BarcodeType type, string data)
    {
        Type = type;
        Data = data;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not NotificationDisplayCode other) return false;
        return Type == other.Type && Data == other.Data;
    }

    public override int GetHashCode() => HashCode.Combine(Type, Data);

    public override string ToString() => $"NotificationDisplayCode{{Type={Type}, Data='{Data}'}}";
}
