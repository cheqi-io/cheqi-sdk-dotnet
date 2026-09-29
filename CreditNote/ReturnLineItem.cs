using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

public sealed class ReturnLineItem
{
    [JsonPropertyName("productId")] public string? ProductId { get; set; }
    [JsonPropertyName("quantity")] public decimal? Quantity { get; set; }
    [JsonPropertyName("reasonCode")] public ReturnReasonCode? ReasonCode { get; set; }
    [JsonPropertyName("reasonDescription")] public string? ReasonDescription { get; set; }

    public void Validate()
    {
        Receipt.ReceiptService.RequireText(ProductId, nameof(ProductId));
        if (ProductId!.Length > 255) throw new ArgumentException("ProductId must be at most 255 characters");
        if (Quantity == null || Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");
        if (ReasonCode == null || !Enum.IsDefined(typeof(ReturnReasonCode), ReasonCode.Value)) throw new ArgumentException("ReasonCode is required");
        if (ReasonDescription?.Length > 1000) throw new ArgumentException("ReasonDescription must be at most 1000 characters");
    }
}
