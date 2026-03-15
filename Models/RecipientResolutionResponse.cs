using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Ubl;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Response from the recipient resolution (customer matching) API.
/// </summary>
public class RecipientResolutionResponse
{
    /// <summary>
    /// Whether matching recipients were found.
    /// </summary>
    [JsonPropertyName("customerFound")]
    public bool customerFound { get; set; }

    /// <summary>
    /// Anonymous match identifier for submitting encrypted receipts.
    /// </summary>
    [JsonPropertyName("matchId")]
    public string? matchId { get; set; }

    /// <summary>
    /// List of matched recipients with their public keys.
    /// </summary>
    [JsonPropertyName("recipients")]
    public List<Recipient>? recipients { get; set; }

    /// <summary>
    /// Payment means information for the customer.
    /// </summary>
    [JsonPropertyName("paymentMeans")]
    public List<PaymentMeans>? paymentMeans { get; set; }

    /// <summary>
    /// When the match identifier expires.
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public DateTimeOffset? expiresAt { get; set; }

    /// <summary>
    /// Instructions for using the match identifier.
    /// </summary>
    [JsonPropertyName("instructions")]
    public string instructions { get; set; } = "Use this anonymous identifier when submitting encrypted receipts. Encrypt receipts using the provided device public keys.";

    /// <summary>
    /// Additional properties.
    /// </summary>
    [JsonPropertyName("additionalProperties")]
    public Dictionary<string, object>? additionalProperties { get; set; }

    // ===== CONSTRUCTORS =====

    /// <summary>
    /// Default constructor for JSON deserialization.
    /// </summary>
    public RecipientResolutionResponse()
    {
        additionalProperties = new Dictionary<string, object>();
    }

    /// <summary>
    /// Private constructor for builder.
    /// </summary>
    private RecipientResolutionResponse(
        bool customerFound,
        string? matchId,
        List<Recipient>? recipients,
        List<PaymentMeans>? paymentMeans,
        DateTimeOffset? expiresAt,
        string? instructions,
        Dictionary<string, object>? additionalProperties)
    {
        this.customerFound = customerFound;
        this.matchId = matchId;
        this.recipients = recipients;
        this.paymentMeans = paymentMeans;
        this.expiresAt = expiresAt;
        this.instructions = instructions ?? "Use this anonymous identifier when submitting encrypted receipts. Encrypt receipts using the provided device public keys.";
        this.additionalProperties = additionalProperties ?? new Dictionary<string, object>();
    }

    [JsonIgnore]
    public bool IsCustomerFound
    {
        get => customerFound;
        set => customerFound = value;
    }

    [JsonIgnore]
    public string? MatchId
    {
        get => matchId;
        set => matchId = value;
    }

    [JsonIgnore]
    public List<Recipient>? Recipients
    {
        get => recipients;
        set => recipients = value;
    }

    [JsonIgnore]
    public List<PaymentMeans>? PaymentMeans
    {
        get => paymentMeans;
        set => paymentMeans = value;
    }

    [JsonIgnore]
    public DateTimeOffset? ExpiresAt
    {
        get => expiresAt;
        set => expiresAt = value;
    }

    [JsonIgnore]
    public string Instructions
    {
        get => instructions;
        set => instructions = value;
    }

    // ===== BUILDER =====

    public static RecipientResolutionResponseBuilder Builder() => new RecipientResolutionResponseBuilder();

    public sealed class RecipientResolutionResponseBuilder
    {
        private bool _customerFound;
        private string? _matchId;
        private List<Recipient>? _recipients;
        private List<PaymentMeans>? _paymentMeans;
        private DateTimeOffset? _expiresAt;
        private string _instructions = "Use this anonymous identifier when submitting encrypted receipts. Encrypt receipts using the provided device public keys.";
        private Dictionary<string, object>? _additionalProperties;

        internal RecipientResolutionResponseBuilder() { }

        public RecipientResolutionResponseBuilder From(RecipientResolutionResponse other)
        {
            _customerFound = other.customerFound;
            _matchId = other.matchId;
            _recipients = other.recipients;
            _paymentMeans = other.paymentMeans;
            _expiresAt = other.expiresAt;
            _instructions = other.instructions;
            _additionalProperties = other.additionalProperties;
            return this;
        }

        public RecipientResolutionResponseBuilder CustomerFound(bool customerFound)
        {
            _customerFound = customerFound;
            return this;
        }

        public RecipientResolutionResponseBuilder MatchId(string? matchId)
        {
            _matchId = matchId;
            return this;
        }

        public RecipientResolutionResponseBuilder Recipients(List<Recipient>? recipients)
        {
            _recipients = recipients;
            return this;
        }

        public RecipientResolutionResponseBuilder PaymentMeans(List<PaymentMeans>? paymentMeans)
        {
            _paymentMeans = paymentMeans;
            return this;
        }

        public RecipientResolutionResponseBuilder ExpiresAt(DateTimeOffset? expiresAt)
        {
            _expiresAt = expiresAt;
            return this;
        }

        public RecipientResolutionResponseBuilder Instructions(string instructions)
        {
            _instructions = instructions;
            return this;
        }

        public RecipientResolutionResponseBuilder AdditionalProperties(Dictionary<string, object>? additionalProperties)
        {
            _additionalProperties = additionalProperties;
            return this;
        }

        public RecipientResolutionResponse Build()
        {
            return new RecipientResolutionResponse(
                _customerFound,
                _matchId,
                _recipients,
                _paymentMeans,
                _expiresAt,
                _instructions,
                _additionalProperties
            );
        }
    }

    // ===== HELPER FACTORY METHODS =====

    /// <summary>
    /// Creates a response indicating no customer was found.
    /// </summary>
    public static RecipientResolutionResponse CustomerNotFound()
    {
        return Builder()
            .CustomerFound(false)
            .Instructions("No customer found with provided payment details")
            .Build();
    }

    /// <summary>
    /// Creates a response indicating no customer was found, with email suggestion.
    /// </summary>
    public static RecipientResolutionResponse CustomerNotFoundWithEmail(string email)
    {
        return Builder()
            .CustomerFound(false)
            .Instructions($"No Cheqi registration found. Consider sending receipt via email to: {email}")
            .Build();
    }

    /// <summary>
    /// Creates a response indicating customer was found with recipients.
    /// </summary>
    public static RecipientResolutionResponse CustomerFound(List<Recipient> recipients)
    {
        return Builder()
            .CustomerFound(true)
            .Recipients(recipients)
            .ExpiresAt(DateTimeOffset.Now.AddDays(1))
            .Build();
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not RecipientResolutionResponse other) return false;
        return customerFound == other.customerFound
            && matchId == other.matchId
            && (recipients == null && other.recipients == null || 
                recipients != null && other.recipients != null && recipients.SequenceEqual(other.recipients))
            && (paymentMeans == null && other.paymentMeans == null ||
                paymentMeans != null && other.paymentMeans != null && paymentMeans.SequenceEqual(other.paymentMeans))
            && expiresAt == other.expiresAt
            && instructions == other.instructions;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(customerFound, matchId, expiresAt, instructions);
    }

    public override string ToString()
    {
        return $"RecipientResolutionResponse{{" +
               $"customerFound={customerFound}, " +
               $"matchId='{matchId}', " +
               $"recipients={recipients?.Count ?? 0} items, " +
               $"paymentMeans={paymentMeans?.Count ?? 0} items, " +
               $"expiresAt={expiresAt}, " +
               $"instructions='{instructions}'}}";
    }
}
