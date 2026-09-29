namespace Cheqi.Sdk.Receipt;

/// <summary>Checkout outcome, separate from the backend processing status.</summary>
public enum ReceiptIssueState { DigitalSubmitted, DigitalPending, DigitalExpired, DownloadFallback, EmailFallback }
