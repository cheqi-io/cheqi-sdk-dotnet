using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Cheqi.Sdk.Models.Company;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Party - represents a party in a transaction (supplier or customer).
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Party
{
    /// <summary>
    /// Party name.
    /// </summary>
    [JsonPropertyName("partyName")]
    [XmlElement("PartyName", Namespace = UblNamespaces.Cac)]
    public string? PartyName { get; set; }

    /// <summary>
    /// Party identification (e.g., VAT number).
    /// </summary>
    [JsonPropertyName("partyIdentification")]
    [XmlElement("PartyIdentification", Namespace = UblNamespaces.Cac)]
    public string? PartyIdentification { get; set; }

    /// <summary>
    /// Identification scheme (e.g., "VAT", "GLN").
    /// </summary>
    [JsonPropertyName("partyIdentificationScheme")]
    [XmlIgnore]
    public string? PartyIdentificationScheme { get; set; }

    /// <summary>
    /// Party postal address.
    /// </summary>
    [JsonPropertyName("postalAddress")]
    [XmlElement("PostalAddress", Namespace = UblNamespaces.Cac)]
    public Address? PostalAddress { get; set; }

    /// <summary>
    /// Party tax scheme.
    /// </summary>
    [JsonPropertyName("partyTaxScheme")]
    [XmlElement("PartyTaxScheme", Namespace = UblNamespaces.Cac)]
    public PartyTaxScheme? PartyTaxScheme { get; set; }

    /// <summary>
    /// Party legal entity information.
    /// </summary>
    [JsonPropertyName("partyLegalEntity")]
    [XmlElement("PartyLegalEntity", Namespace = UblNamespaces.Cac)]
    public PartyLegalEntity? PartyLegalEntity { get; set; }

    /// <summary>
    /// Contact information.
    /// </summary>
    [JsonPropertyName("contact")]
    [XmlElement("Contact", Namespace = UblNamespaces.Cac)]
    public Contact? Contact { get; set; }
}

/// <summary>
/// UBL Party Tax Scheme.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PartyTaxScheme
{
    /// <summary>
    /// Company ID (VAT number).
    /// </summary>
    [JsonPropertyName("companyId")]
    [XmlElement("CompanyID", Namespace = UblNamespaces.Cbc)]
    public string? CompanyId { get; set; }

    /// <summary>
    /// Tax scheme ID (e.g., "VAT").
    /// </summary>
    [JsonPropertyName("taxSchemeId")]
    [XmlElement("TaxScheme", Namespace = UblNamespaces.Cac)]
    public string? TaxSchemeId { get; set; }
}

/// <summary>
/// UBL Party Legal Entity.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PartyLegalEntity
{
    /// <summary>
    /// Registration name.
    /// </summary>
    [JsonPropertyName("registrationName")]
    [XmlElement("RegistrationName", Namespace = UblNamespaces.Cbc)]
    public string? RegistrationName { get; set; }

    /// <summary>
    /// Company ID (registration number).
    /// </summary>
    [JsonPropertyName("companyId")]
    [XmlElement("CompanyID", Namespace = UblNamespaces.Cbc)]
    public string? CompanyId { get; set; }

    /// <summary>
    /// Company ID scheme.
    /// </summary>
    [JsonPropertyName("companyIdScheme")]
    [XmlIgnore]
    public string? CompanyIdScheme { get; set; }
}

/// <summary>
/// UBL Contact information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Contact
{
    /// <summary>
    /// Contact name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public string? Name { get; set; }

    /// <summary>
    /// Telephone number.
    /// </summary>
    [JsonPropertyName("telephone")]
    [XmlElement("Telephone", Namespace = UblNamespaces.Cbc)]
    public string? Telephone { get; set; }

    /// <summary>
    /// Email address.
    /// </summary>
    [JsonPropertyName("electronicMail")]
    [XmlElement("ElectronicMail", Namespace = UblNamespaces.Cbc)]
    public string? ElectronicMail { get; set; }
}
