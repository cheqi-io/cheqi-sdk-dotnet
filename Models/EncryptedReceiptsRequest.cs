using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Request containing multiple encrypted receipts for batch submission.
/// </summary>
public sealed class EncryptedReceiptsRequest
{
    /// <summary>
    /// Set of encrypted receipts to submit.
    /// </summary>
    [JsonPropertyName("encryptedReceipts")]
    public IReadOnlySet<EncryptedReceiptRequestDto> encryptedReceipts { get; }

    /// <summary>
    /// Match ID for tracking this batch of receipts.
    /// </summary>
    [JsonPropertyName("matchId")]
    public string matchId { get; }

    /// <summary>
    /// Template hash for the receipt template used.
    /// </summary>
    [JsonPropertyName("templateHash")]
    public string templateHash { get; }

    // ===== CONSTRUCTOR =====

    private EncryptedReceiptsRequest(
        string matchId,
        ISet<EncryptedReceiptRequestDto> encryptedReceipts,
        string templateHash)
    {
        this.matchId = matchId;
        this.encryptedReceipts = encryptedReceipts?.ToHashSet() ?? new HashSet<EncryptedReceiptRequestDto>();
        this.templateHash = templateHash;
    }

    // ===== BUILDER =====

    public static EncryptedReceiptsRequestBuilder Builder() => new EncryptedReceiptsRequestBuilder();

    public sealed class EncryptedReceiptsRequestBuilder
    {
        private ISet<EncryptedReceiptRequestDto>? _encryptedReceipts;
        private string? _matchId;
        private string? _templateHash;

        internal EncryptedReceiptsRequestBuilder() { }

        public EncryptedReceiptsRequestBuilder From(EncryptedReceiptsRequest other)
        {
            _encryptedReceipts = other.encryptedReceipts.ToHashSet();
            _matchId = other.matchId;
            _templateHash = other.templateHash;
            return this;
        }

        public EncryptedReceiptsRequestBuilder EncryptedReceipts(ISet<EncryptedReceiptRequestDto> encryptedReceipts)
        {
            _encryptedReceipts = encryptedReceipts;
            return this;
        }

        public EncryptedReceiptsRequestBuilder AddEncryptedReceipt(EncryptedReceiptRequestDto receipt)
        {
            _encryptedReceipts ??= new HashSet<EncryptedReceiptRequestDto>();
            _encryptedReceipts.Add(receipt);
            return this;
        }

        public EncryptedReceiptsRequestBuilder MatchId(string matchId)
        {
            _matchId = matchId;
            return this;
        }

        public EncryptedReceiptsRequestBuilder TemplateHash(string templateHash)
        {
            _templateHash = templateHash;
            return this;
        }

        public EncryptedReceiptsRequest Build()
        {
            if (_matchId == null)
                throw new InvalidOperationException("MatchId is required");
            if (_encryptedReceipts == null || _encryptedReceipts.Count == 0)
                throw new InvalidOperationException("At least one encrypted receipt is required");
            if (_templateHash == null)
                throw new InvalidOperationException("TemplateHash is required");

            return new EncryptedReceiptsRequest(_matchId, _encryptedReceipts, _templateHash);
        }
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not EncryptedReceiptsRequest other) return false;
        return encryptedReceipts.SetEquals(other.encryptedReceipts)
            && matchId == other.matchId
            && templateHash == other.templateHash;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(matchId);
        hash.Add(templateHash);
        foreach (var receipt in encryptedReceipts.OrderBy(r => r.receiptId))
        {
            hash.Add(receipt);
        }
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        return $"EncryptedReceiptsRequest{{" +
               $"encryptedReceipts={encryptedReceipts.Count} receipts, " +
               $"matchId='{matchId}', " +
               $"templateHash='{templateHash}'}}";
    }
}
