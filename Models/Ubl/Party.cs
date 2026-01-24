using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Company;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Party - represents a party in a transaction (supplier or customer).
/// </summary>
public sealed class Party
{
    /// <summary>
    /// Party name.
    /// </summary>
    [JsonPropertyName("partyName")]
    public string? PartyName { get; set; }

    /// <summary>
    /// Party identification (e.g., VAT number).
    /// </summary>
    [JsonPropertyName("partyIdentification")]
    public string? PartyIdentification { get; set; }

    /// <summary>
    /// Identification scheme (e.g., "VAT", "GLN").
    /// </summary>
    [JsonPropertyName("partyIdentificationScheme")]
    public string? PartyIdentificationScheme { get; set; }

    /// <summary>
    /// Party postal address.
    /// </summary>
    [JsonPropertyName("postalAddress")]
    public Address? PostalAddress { get; set; }

    /// <summary>
    /// Party tax scheme.
    /// </summary>
    [JsonPropertyName("partyTaxScheme")]
    public PartyTaxScheme? PartyTaxScheme { get; set; }

    /// <summary>
    /// Party legal entity information.
    /// </summary>
    [JsonPropertyName("partyLegalEntity")]
    public PartyLegalEntity? PartyLegalEntity { get; set; }

    /// <summary>
    /// Contact information.
    /// </summary>
    [JsonPropertyName("contact")]
    public Contact? Contact { get; set; }
}

/// <summary>
/// UBL Party Tax Scheme.
/// </summary>
public sealed class PartyTaxScheme
{
    /// <summary>
    /// Company ID (VAT number).
    /// </summary>
    [JsonPropertyName("companyId")]
    public string? CompanyId { get; set; }

    /// <summary>
    /// Tax scheme ID (e.g., "VAT").
    /// </summary>
    [JsonPropertyName("taxSchemeId")]
    public string? TaxSchemeId { get; set; }
}

/// <summary>
/// UBL Party Legal Entity.
/// </summary>
public sealed class PartyLegalEntity
{
    /// <summary>
    /// Registration name.
    /// </summary>
    [JsonPropertyName("registrationName")]
    public string? RegistrationName { get; set; }

    /// <summary>
    /// Company ID (registration number).
    /// </summary>
    [JsonPropertyName("companyId")]
    public string? CompanyId { get; set; }

    /// <summary>
    /// Company ID scheme.
    /// </summary>
    [JsonPropertyName("companyIdScheme")]
    public string? CompanyIdScheme { get; set; }
}

/// <summary>
/// UBL Contact information.
/// </summary>
public sealed class Contact
{
    /// <summary>
    /// Contact name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Telephone number.
    /// </summary>
    [JsonPropertyName("telephone")]
    public string? Telephone { get; set; }

    /// <summary>
    /// Email address.
    /// </summary>
    [JsonPropertyName("electronicMail")]
    public string? ElectronicMail { get; set; }
}
