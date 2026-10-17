namespace Cheqi.Sdk.Http;

/// <summary>Endpoint paths for the current Cheqi API.</summary>
public static class Endpoints
{
    public const string ReceiptDownload = "/receipt/download";
    public const string ReceiptEncrypted = "/receipt/encrypted";
    public const string ReceiptEmail = "/receipt/email";
    public const string RecipientResolve = "/recipient/resolve";
    public const string CreditNoteEncrypted = "/credit-note/encrypted";
    public const string OAuthToken = "/oauth2/token";
    public const string ReceiptDestinations = "/company/receipt-destinations";
    public static string ReceiptDestination(Guid id) => $"{ReceiptDestinations}/{id:D}";
    public static string ReceiptDestinationQueue(Guid id) => ReceiptDestination(id) + "/receipts/queue";
    public static string ReceiptDestinationAcknowledge(Guid id) => ReceiptDestinationQueue(id) + "/acknowledge";
    public static string WebhookSubscription(Guid id) => $"/webhook/subscription/{id:D}";
    public static string MatchStatus(string matchId) => $"/recipient/matches/{Uri.EscapeDataString(matchId)}";
    public static string CompanyStores(Guid companyId) => $"/company/{companyId:D}/stores";
    public static string CompanyInviteEmployees(Guid companyId) => $"/company/{companyId:D}/invite/employees";
    public static string CompanyStore(Guid companyId, Guid storeId) => $"/company/{companyId:D}/stores/{storeId:D}";
    public static string CompanyStoreActivate(Guid companyId, Guid storeId) => CompanyStore(companyId, storeId) + "/activate";
    public static string CompanyStoreDeactivate(Guid companyId, Guid storeId) => CompanyStore(companyId, storeId) + "/deactivate";
}
