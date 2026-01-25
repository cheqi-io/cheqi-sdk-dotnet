using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

public class RefundBankAccount
{
    [JsonPropertyName("iban")]
    public string? Iban { get; set; }
    
    [JsonPropertyName("bic")]
    public string? Bic { get; set; }
    
    [JsonPropertyName("accountHolder")]
    public string? AccountHolder { get; set; }

    public RefundBankAccount()
    {
    }

    public RefundBankAccount(string iban, string bic, string accountHolder)
    {
        Iban = iban;
        Bic = bic;
        AccountHolder = accountHolder;
    }
}
