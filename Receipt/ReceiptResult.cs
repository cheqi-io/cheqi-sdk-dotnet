using System.Text.Json.Serialization;
using Cheqi.Sdk.Models;
using ReceiptCreatedResponse = Cheqi.Sdk.Models.Generated.ReceiptCreatedResponse;

namespace Cheqi.Sdk.Receipt;

/// <summary>
/// Result of processing a receipt through the SDK.
/// </summary>
public class ReceiptResult
{
    [JsonPropertyName("success")]
    public bool Success { get; }
    
    [JsonPropertyName("deliveryStatus")]
    public DeliveryStatus DeliveryStatus { get; }
    
    [JsonPropertyName("cheqiReceiptId")]
    public string? CheqiReceiptId { get; }
    
    [JsonPropertyName("createdAt")]
    public DateTimeOffset? CreatedAt { get; }
    
    [JsonPropertyName("templateHash")]
    public string? TemplateHash { get; }
    
    [JsonPropertyName("canonicalJson")]
    public string? CanonicalJson { get; }
    
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; }
    
    [JsonPropertyName("message")]
    public string? Message { get; }

    private ReceiptResult(
        bool success,
        DeliveryStatus deliveryStatus,
        string? cheqiReceiptId,
        DateTimeOffset? createdAt,
        string? templateHash,
        string? canonicalJson,
        string? emailAddress,
        string? message)
    {
        Success = success;
        DeliveryStatus = deliveryStatus;
        CheqiReceiptId = cheqiReceiptId;
        CreatedAt = createdAt;
        TemplateHash = templateHash;
        CanonicalJson = canonicalJson;
        EmailAddress = emailAddress;
        Message = message;
    }

    public static ReceiptResult DeliveredToApp(ReceiptCreatedResponse response, string canonicalJson)
    {
        return new ReceiptResult(
            true,
            DeliveryStatus.DELIVERED_DIGITAL,
            response.CheqiReceiptId,
            response.CreatedAt,
            response.TemplateHash,
            canonicalJson,
            null,
            "Receipt delivered to customer's Cheqi app"
        );
    }

    public static ReceiptResult DeliveredViaEmail(string emailAddress)
    {
        return new ReceiptResult(
            true,
            DeliveryStatus.DELIVERED_EMAIL,
            null,
            DateTimeOffset.Now,
            null,
            null,
            emailAddress,
            $"Receipt sent via email to {emailAddress}"
        );
    }

    public static ReceiptResult CustomerNotFound()
    {
        return new ReceiptResult(
            false,
            DeliveryStatus.CUSTOMER_NOT_FOUND,
            null,
            null,
            null,
            null,
            null,
            "Customer not found with provided payment details"
        );
    }

    public static ReceiptResult Failed(string message)
    {
        return new ReceiptResult(
            false,
            DeliveryStatus.FAILED,
            null,
            null,
            null,
            null,
            null,
            message
        );
    }

    // Core status checks
    public bool IsSuccess() => Success;
    public bool IsCustomerNotFound() => DeliveryStatus == DeliveryStatus.CUSTOMER_NOT_FOUND;

    public override string ToString()
    {
        return $"ReceiptResult{{success={Success}, deliveryStatus={DeliveryStatus}, cheqiReceiptId='{CheqiReceiptId}'}}";
    }
}
