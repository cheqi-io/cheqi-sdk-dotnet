using System.Security.Cryptography;
using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Tests.TestSupport;

internal static class TestData
{
    public static IdentificationDetails PairingCodeIdentification(string pairingCode = "PAIR-123") =>
        IdentificationDetails.ForPairingCode(PaymentType.CARD_PAYMENT, pairingCode);

    public static IdentificationDetails ReceiptIdIdentification(string cheqiReceiptId = "receipt-123", string? email = null) =>
        IdentificationDetails.ForReceiptId(PaymentType.CARD_PAYMENT, cheqiReceiptId, email);

    public static IdentificationDetails EmailIdentification(string email = "customer@example.com") =>
        IdentificationDetails.ForEmail(PaymentType.CARD_PAYMENT, email);

    public static RecipientResolutionResponse CustomerFoundResponse(string matchId = "match-123", List<Recipient>? recipients = null) =>
        RecipientResolutionResponse.Builder()
            .CustomerFound(true)
            .MatchId(matchId)
            .Recipients(recipients ?? new List<Recipient> { CreateRecipient() })
            .Build();

    public static Recipient CreateRecipient(string id = "recipient-1")
    {
        using var rsa = RSA.Create(2048);
        var publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());

        return new Recipient(
            id,
            ReceiverType.DEVICE,
            publicKey,
            "RSA",
            DateTimeOffset.UtcNow);
    }

    public static ReceiptTemplateRequest CreateReceiptTemplateRequest(string documentNumber = "INV-001") =>
        new ReceiptTemplateRequest(
            documentNumber: documentNumber,
            issueDate: DateTimeOffset.Parse("2026-03-15T10:00:00Z"),
            currency: "EUR",
            receiptSubtotal: 100m,
            totalBeforeTax: 100m,
            totalTaxAmount: 21m,
            totalAmount: 121m,
            products: new[]
            {
                Product.Builder()
                    .Identifier("SKU-1")
                    .Brand("Cheqi")
                    .Name("Subscription")
                    .Quantity(1)
                    .BaseQuantity(1)
                    .UnitCode(UnitCode.ONE)
                    .UnitPrice(100m)
                    .Subtotal(100m)
                    .Total(121m)
                    .AddTax(21.0, "VAT", 21m)
                    .Build()
            },
            taxes: new[]
            {
                Tax.Builder()
                    .Rate(21.0)
                    .Type("VAT")
                    .Amount(21m)
                    .Build()
            });

    public static CreditNoteTemplateRequest CreateCreditNoteTemplateRequest(string documentNumber = "CN-001", string originatorDocumentReference = "INV-001") =>
        CreditNoteTemplateRequest.Builder()
            .DocumentNumber(documentNumber)
            .OriginatorDocumentReference(originatorDocumentReference)
            .IssueDate(DateTimeOffset.Parse("2026-03-15T10:00:00Z"))
            .Currency("EUR")
            .CreditNoteSubTotal(-100m)
            .TotalBeforeTax(-100m)
            .TotalTaxAmount(-21m)
            .TotalAmount(-121m)
            .AddProduct("Cheqi", "Subscription refund", 100m, -100m, 21.0, -21m, -121m, -1.0)
            .AddTax(21.0, "VAT", -21m)
            .Build();
}
