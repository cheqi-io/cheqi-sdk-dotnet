using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Ubl;

namespace Cheqi.Sdk.Http;

/// <summary>
/// Request DTO for sending a receipt via email.
/// Contains the customer email address and the purchase receipt data.
/// </summary>
public class EmailReceiptRequest
{
    [JsonPropertyName("customerEmail")]
    public string CustomerEmail { get; }

    [JsonPropertyName("purchaseReceipt")]
    public PurchaseReceipt PurchaseReceipt { get; }

    public EmailReceiptRequest(string customerEmail, PurchaseReceipt purchaseReceipt)
    {
        CustomerEmail = customerEmail;
        PurchaseReceipt = purchaseReceipt;
    }
}
