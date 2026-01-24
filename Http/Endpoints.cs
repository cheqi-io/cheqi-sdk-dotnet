namespace Cheqi.Sdk.Http;

/// <summary>
/// API endpoint paths for the Cheqi API.
/// </summary>
public static class Endpoints
{
    /// <summary>
    /// Receipt template generation endpoint.
    /// </summary>
    public const string ReceiptTemplate = "/receipt/template";

    /// <summary>
    /// Encrypted receipt submission endpoint.
    /// </summary>
    public const string ReceiptEncrypted = "/receipt/encrypted";

    /// <summary>
    /// Email receipt delivery endpoint.
    /// </summary>
    public const string ReceiptEmail = "/receipt/email";

    /// <summary>
    /// Recipient resolution (customer matching) endpoint.
    /// </summary>
    public const string RecipientResolve = "/recipient/resolve";

    /// <summary>
    /// Credit note template generation endpoint.
    /// </summary>
    public const string CreditNoteTemplate = "/credit-note/template";

    /// <summary>
    /// Encrypted credit note submission endpoint.
    /// </summary>
    public const string CreditNoteEncrypted = "/credit-note/encrypted";

    /// <summary>
    /// Company provisioning endpoint.
    /// </summary>
    public const string CompanyProvision = "/company/provision";

    /// <summary>
    /// Gets the stores endpoint for a specific company.
    /// </summary>
    public static string CompanyStores(string companyId) => $"/company/{companyId}/stores";

    /// <summary>
    /// Gets the store endpoint for a specific store.
    /// </summary>
    public static string CompanyStore(string companyId, string storeId) => $"/company/{companyId}/stores/{storeId}";

    /// <summary>
    /// Gets the store activation endpoint.
    /// </summary>
    public static string CompanyStoreActivate(string companyId, string storeId) => $"/company/{companyId}/stores/{storeId}/activate";

    /// <summary>
    /// Gets the store deactivation endpoint.
    /// </summary>
    public static string CompanyStoreDeactivate(string companyId, string storeId) => $"/company/{companyId}/stores/{storeId}/deactivate";

    /// <summary>
    /// OAuth token endpoint.
    /// </summary>
    public const string OAuthToken = "/oauth/token";
}
