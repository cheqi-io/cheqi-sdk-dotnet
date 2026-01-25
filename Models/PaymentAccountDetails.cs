using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Payment account details DTO representing payment account information.
///
/// <para><b>Mandatory Fields:</b></para>
/// <list type="bullet">
///   <item><b>identifier</b>: The account identifier (IBAN, account number, etc.)</item>
///   <item><b>type</b>: The type of account identifier</item>
/// </list>
/// </summary>
public sealed class PaymentAccountDetails
{
    // ===== MANDATORY FIELDS =====

    /// <summary>
    /// The account identifier (IBAN, account number, wallet ID, etc.).
    /// </summary>
    [JsonPropertyName("identifier")]
    public string identifier { get; }

    /// <summary>
    /// The type of account identifier.
    /// </summary>
    [JsonPropertyName("accountIdentifierType")]
    public AccountIdentifierType type { get; }

    // ===== OPTIONAL FIELDS =====

    /// <summary>
    /// Bank Identifier Code (SWIFT/BIC) - for IBAN payments.
    /// </summary>
    [JsonPropertyName("bic")]
    public string? bic { get; }

    /// <summary>
    /// Bank name - for bank accounts.
    /// </summary>
    [JsonPropertyName("bankName")]
    public string? bankName { get; }

    /// <summary>
    /// Account holder name - for bank accounts.
    /// </summary>
    [JsonPropertyName("accountHolderName")]
    public string? accountHolderName { get; }

    /// <summary>
    /// Institution name - universal (bank, wallet provider, etc.).
    /// </summary>
    [JsonPropertyName("institutionName")]
    public string? institutionName { get; }

    // ===== CONSTRUCTOR =====

    private PaymentAccountDetails(
        string identifier,
        AccountIdentifierType type,
        string? bic,
        string? bankName,
        string? accountHolderName,
        string? institutionName)
    {
        this.identifier = identifier;
        this.type = type;
        this.bic = bic;
        this.bankName = bankName;
        this.accountHolderName = accountHolderName;
        this.institutionName = institutionName;
    }

    // ===== BUILDER =====

    public static PaymentAccountDetailsBuilder Builder() => new PaymentAccountDetailsBuilder();

    public sealed class PaymentAccountDetailsBuilder
    {
        private string? _identifier;
        private AccountIdentifierType? _type;
        private string? _bic;
        private string? _bankName;
        private string? _accountHolderName;
        private string? _institutionName;

        internal PaymentAccountDetailsBuilder() { }

        public PaymentAccountDetailsBuilder From(PaymentAccountDetails other)
        {
            _identifier = other.identifier;
            _type = other.type;
            _bic = other.bic;
            _bankName = other.bankName;
            _accountHolderName = other.accountHolderName;
            _institutionName = other.institutionName;
            return this;
        }

        public PaymentAccountDetailsBuilder Identifier(string identifier)
        {
            _identifier = identifier;
            return this;
        }

        public PaymentAccountDetailsBuilder Type(AccountIdentifierType type)
        {
            _type = type;
            return this;
        }

        public PaymentAccountDetailsBuilder Bic(string? bic)
        {
            _bic = bic;
            return this;
        }

        public PaymentAccountDetailsBuilder BankName(string? bankName)
        {
            _bankName = bankName;
            return this;
        }

        public PaymentAccountDetailsBuilder AccountHolderName(string? accountHolderName)
        {
            _accountHolderName = accountHolderName;
            return this;
        }

        public PaymentAccountDetailsBuilder InstitutionName(string? institutionName)
        {
            _institutionName = institutionName;
            return this;
        }

        public PaymentAccountDetails Build()
        {
            if (_identifier == null)
                throw new InvalidOperationException("Identifier is required");
            if (_type == null)
                throw new InvalidOperationException("Type is required");

            return new PaymentAccountDetails(
                _identifier,
                _type.Value,
                _bic,
                _bankName,
                _accountHolderName,
                _institutionName
            );
        }
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not PaymentAccountDetails other) return false;
        return identifier == other.identifier
            && type == other.type
            && bic == other.bic
            && bankName == other.bankName
            && accountHolderName == other.accountHolderName
            && institutionName == other.institutionName;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(identifier, type);
    }

    public override string ToString()
    {
        return $"PaymentAccountDetails{{" +
               $"identifier='{identifier}', " +
               $"type={type}, " +
               $"bic='{bic}', " +
               $"bankName='{bankName}', " +
               $"accountHolderName='{accountHolderName}', " +
               $"institutionName='{institutionName}'}}";
    }
}
