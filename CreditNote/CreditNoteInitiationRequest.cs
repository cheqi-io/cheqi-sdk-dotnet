using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

/// <summary>Plaintext customer return request, decrypted and validated locally by the merchant.</summary>
public sealed class CreditNoteInitiationRequest
{
    [JsonPropertyName("cheqiReceiptId")] public string? CheqiReceiptId { get; set; }
    [JsonPropertyName("receiptId")] public string? ReceiptId { get; set; }
    [JsonPropertyName("customerNote")] public string? CustomerNote { get; set; }
    [JsonPropertyName("lineItems")] public List<ReturnLineItem>? LineItems { get; set; }
    [JsonPropertyName("refundPreference")] public RefundPreference? RefundPreference { get; set; }
    [JsonPropertyName("refundBankAccount")] public RefundBankAccount? RefundBankAccount { get; set; }

    public void Validate()
    {
        Receipt.ReceiptService.RequireText(CheqiReceiptId, nameof(CheqiReceiptId));
        Receipt.ReceiptService.RequireText(ReceiptId, nameof(ReceiptId));
        if (CustomerNote?.Length > 1000) throw new ArgumentException("CustomerNote must be at most 1000 characters");
        if (LineItems == null || LineItems.Count == 0 || LineItems.Any(item => item == null)) throw new ArgumentException("At least one return line item is required");
        foreach (var item in LineItems) item.Validate();
        if (RefundPreference == null || !Enum.IsDefined(typeof(RefundPreference), RefundPreference.Value)) throw new ArgumentException("RefundPreference is required");
        if (RefundPreference == CreditNote.RefundPreference.BANK_TRANSFER)
        {
            if (RefundBankAccount == null) throw new ArgumentException("RefundBankAccount is required for BANK_TRANSFER");
            Receipt.ReceiptService.RequireText(RefundBankAccount.Iban, "refundBankAccount.iban");
            Receipt.ReceiptService.RequireText(RefundBankAccount.AccountHolder, "refundBankAccount.accountHolder");
        }
        else if (RefundBankAccount != null) throw new ArgumentException("RefundBankAccount is only allowed for BANK_TRANSFER");
    }
}
