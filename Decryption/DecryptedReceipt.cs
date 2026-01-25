namespace Cheqi.Sdk.Decryption;

public class DecryptedReceipt
{
    public string ReceiptContentJson { get; }
    public string? CustomerDetails { get; }

    public DecryptedReceipt(string receiptContentJson, string? customerDetails)
    {
        ReceiptContentJson = receiptContentJson;
        CustomerDetails = customerDetails;
    }

    public bool HasCustomerDetails()
    {
        return !string.IsNullOrWhiteSpace(CustomerDetails);
    }
}
