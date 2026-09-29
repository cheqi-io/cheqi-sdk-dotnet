using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Party - represents a party in a transaction (supplier or customer).
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Party
{
    /// <summary>
    /// Endpoint ID.
    /// </summary>
    [JsonPropertyName("endpointID")]
    [XmlElement("EndpointID", Namespace = UblNamespaces.Cbc)]
    public Identifier? EndpointId { get; set; }

    /// <summary>
    /// Party identification (e.g., VAT number).
    /// </summary>
    [JsonPropertyName("partyIdentification")]
    [XmlElement("PartyIdentification", Namespace = UblNamespaces.Cac)]
    public List<PartyIdentification>? PartyIdentification { get; set; }

    /// <summary>
    /// Party name.
    /// </summary>
    [JsonPropertyName("partyName")]
    [XmlElement("PartyName", Namespace = UblNamespaces.Cac)]
    public PartyName? PartyName { get; set; }

    /// <summary>
    /// Party postal address.
    /// </summary>
    [JsonPropertyName("postalAddress")]
    [XmlElement("PostalAddress", Namespace = UblNamespaces.Cac)]
    public Address? PostalAddress { get; set; }

    /// <summary>
    /// Party tax schemes.
    /// </summary>
    [JsonPropertyName("partyTaxScheme")]
    [XmlElement("PartyTaxScheme", Namespace = UblNamespaces.Cac)]
    public List<PartyTaxScheme>? PartyTaxScheme { get; set; }

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
    [JsonPropertyName("companyID")]
    [XmlElement("CompanyID", Namespace = UblNamespaces.Cbc)]
    public string? CompanyId { get; set; }

    /// <summary>
    /// Tax scheme.
    /// </summary>
    [JsonPropertyName("taxScheme")]
    [XmlElement("TaxScheme", Namespace = UblNamespaces.Cac)]
    public TaxScheme? TaxScheme { get; set; }
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
    [JsonPropertyName("companyID")]
    [XmlElement("CompanyID", Namespace = UblNamespaces.Cbc)]
    public Identifier? CompanyId { get; set; }
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
