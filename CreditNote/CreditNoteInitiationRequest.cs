using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

public class CreditNoteInitiationRequest
{
    [JsonPropertyName("cheqiReceiptId")]
    public string? CheqiReceiptId { get; set; }
    
    [JsonPropertyName("receiptId")]
    public string? ReceiptId { get; set; }
    
    [JsonPropertyName("returnReason")]
    public string? ReturnReason { get; set; }
    
    [JsonPropertyName("returnReasonDescription")]
    public string? ReturnReasonDescription { get; set; }
    
    [JsonPropertyName("lineItems")]
    public List<ReturnLineItem>? LineItems { get; set; }
    
    [JsonPropertyName("refundPreference")]
    public RefundPreference? RefundPreference { get; set; }
    
    [JsonPropertyName("refundBankAccount")]
    public RefundBankAccount? RefundBankAccount { get; set; }

    public CreditNoteInitiationRequest()
    {
    }

    private CreditNoteInitiationRequest(Builder builder)
    {
        CheqiReceiptId = builder._cheqiReceiptId;
        ReceiptId = builder._receiptId;
        ReturnReason = builder._returnReason;
        ReturnReasonDescription = builder._returnReasonDescription;
        LineItems = builder._lineItems;
        RefundPreference = builder._refundPreference;
        RefundBankAccount = builder._refundBankAccount;
    }

    public static Builder BuilderInstance() => new Builder();

    public class Builder
    {
        internal string? _cheqiReceiptId;
        internal string? _receiptId;
        internal string? _returnReason;
        internal string? _returnReasonDescription;
        internal List<ReturnLineItem>? _lineItems;
        internal RefundPreference? _refundPreference;
        internal RefundBankAccount? _refundBankAccount;

        public Builder CheqiReceiptId(string cheqiReceiptId)
        {
            _cheqiReceiptId = cheqiReceiptId;
            return this;
        }

        public Builder ReceiptId(string receiptId)
        {
            _receiptId = receiptId;
            return this;
        }

        public Builder ReturnReason(string returnReason)
        {
            _returnReason = returnReason;
            return this;
        }

        public Builder ReturnReasonDescription(string returnReasonDescription)
        {
            _returnReasonDescription = returnReasonDescription;
            return this;
        }

        public Builder LineItems(List<ReturnLineItem> lineItems)
        {
            _lineItems = lineItems;
            return this;
        }

        public Builder RefundPreference(RefundPreference refundPreference)
        {
            _refundPreference = refundPreference;
            return this;
        }

        public Builder RefundBankAccount(RefundBankAccount refundBankAccount)
        {
            _refundBankAccount = refundBankAccount;
            return this;
        }

        public CreditNoteInitiationRequest Build()
        {
            return new CreditNoteInitiationRequest(this);
        }
    }
}
