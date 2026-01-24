namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL XML namespace constants.
/// </summary>
public static class UblNamespaces
{
    /// <summary>
    /// UBL Invoice namespace.
    /// </summary>
    public const string Invoice = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";

    /// <summary>
    /// UBL Credit Note namespace.
    /// </summary>
    public const string CreditNote = "urn:oasis:names:specification:ubl:schema:xsd:CreditNote-2";

    /// <summary>
    /// Cheqi Purchase Receipt namespace.
    /// </summary>
    public const string PurchaseReceipt = "urn:cheqi:names:specification:ubl:schema:xsd:PurchaseReceipt-1";

    /// <summary>
    /// Common Basic Components namespace (for simple elements like ID, Name, etc.).
    /// </summary>
    public const string Cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";

    /// <summary>
    /// Common Aggregate Components namespace (for complex elements like Party, Address, etc.).
    /// </summary>
    public const string Cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";
}
