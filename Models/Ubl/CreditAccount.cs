using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Credit Account - a credit account.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class CreditAccount
{
    /// <summary>
    /// Account identifier.
    /// </summary>
    [JsonPropertyName("accountId")]
    [XmlElement("AccountID", Namespace = UblNamespaces.Cbc)]
    public Identifier? AccountId { get; set; }
}
