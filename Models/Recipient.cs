using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Key information DTO representing cryptographic key details.
///
/// <para><b>Mandatory Fields:</b></para>
/// <list type="bullet">
///   <item><b>id</b>: Unique recipient identifier</item>
///   <item><b>receiverType</b>: Type of receiver (DEVICE or WEBHOOK)</item>
///   <item><b>publicKey</b>: The public key string</item>
///   <item><b>keyAlgorithm</b>: The algorithm used for the key</item>
/// </list>
///
/// <para><b>Optional Fields:</b></para>
/// <list type="bullet">
///   <item><b>keyCreatedAt</b>: When the key was created</item>
/// </list>
/// </summary>
public sealed class Recipient
{
    // ===== MANDATORY FIELDS =====

    /// <summary>
    /// Unique identifier for the recipient.
    /// </summary>
    [JsonPropertyName("id")]
    public string id { get; }

    /// <summary>
    /// Type of receiver (DEVICE or WEBHOOK).
    /// </summary>
    [JsonPropertyName("receiverType")]
    public ReceiverType receiverType { get; }

    /// <summary>
    /// The public key string.
    /// </summary>
    [JsonPropertyName("publicKey")]
    public string publicKey { get; }

    /// <summary>
    /// The algorithm used for the key.
    /// </summary>
    [JsonPropertyName("keyAlgorithm")]
    public string keyAlgorithm { get; }

    // ===== OPTIONAL FIELDS =====

    /// <summary>
    /// When the key was created.
    /// </summary>
    [JsonPropertyName("keyCreatedAt")]
    public DateTimeOffset? keyCreatedAt { get; }

    // ===== CONSTRUCTOR =====

    [JsonConstructor]
    public Recipient(
        string id,
        ReceiverType receiverType,
        string publicKey,
        string keyAlgorithm,
        DateTimeOffset? keyCreatedAt)
    {
        this.id = id;
        this.receiverType = receiverType;
        this.publicKey = publicKey;
        this.keyAlgorithm = keyAlgorithm;
        this.keyCreatedAt = keyCreatedAt;
    }

    // ===== BUILDER =====

    public static RecipientBuilder Builder() => new RecipientBuilder();

    public sealed class RecipientBuilder
    {
        private string? _id;
        private ReceiverType? _receiverType;
        private string? _publicKey;
        private string? _keyAlgorithm;
        private DateTimeOffset? _keyCreatedAt;

        internal RecipientBuilder() { }

        public RecipientBuilder From(Recipient other)
        {
            _id = other.id;
            _receiverType = other.receiverType;
            _publicKey = other.publicKey;
            _keyAlgorithm = other.keyAlgorithm;
            _keyCreatedAt = other.keyCreatedAt;
            return this;
        }

        public RecipientBuilder Id(string id)
        {
            _id = id;
            return this;
        }

        public RecipientBuilder ReceiverType(ReceiverType receiverType)
        {
            _receiverType = receiverType;
            return this;
        }

        public RecipientBuilder PublicKey(string publicKey)
        {
            _publicKey = publicKey;
            return this;
        }

        public RecipientBuilder KeyAlgorithm(string keyAlgorithm)
        {
            _keyAlgorithm = keyAlgorithm;
            return this;
        }

        public RecipientBuilder KeyCreatedAt(DateTimeOffset? keyCreatedAt)
        {
            _keyCreatedAt = keyCreatedAt;
            return this;
        }

        public Recipient Build()
        {
            if (_id == null)
                throw new InvalidOperationException("Id is required");
            if (_receiverType == null)
                throw new InvalidOperationException("ReceiverType is required");
            if (_publicKey == null)
                throw new InvalidOperationException("PublicKey is required");
            if (_keyAlgorithm == null)
                throw new InvalidOperationException("KeyAlgorithm is required");

            return new Recipient(
                _id,
                _receiverType.Value,
                _publicKey,
                _keyAlgorithm,
                _keyCreatedAt
            );
        }
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not Recipient other) return false;
        return id == other.id
            && receiverType == other.receiverType
            && publicKey == other.publicKey
            && keyAlgorithm == other.keyAlgorithm
            && keyCreatedAt == other.keyCreatedAt;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(id, receiverType, publicKey, keyAlgorithm, keyCreatedAt);
    }

    public override string ToString()
    {
        return $"Recipient{{" +
               $"id='{id}', " +
               $"receiverType='{receiverType}', " +
               $"publicKey='{publicKey}', " +
               $"keyAlgorithm='{keyAlgorithm}', " +
               $"keyCreatedAt={keyCreatedAt}}}";
    }
}
