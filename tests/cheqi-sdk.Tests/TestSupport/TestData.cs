using System.Security.Cryptography;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Enums;
using CreditNoteTemplateRequest = Cheqi.Sdk.CreditNote.CreditNoteTemplateRequest;
using ReceiptTemplateRequest = Cheqi.Sdk.Models.ReceiptTemplateRequest;
using IdentificationDetails = Cheqi.Sdk.Models.Generated.IdentificationDetails;
using MatchedRecipient = Cheqi.Sdk.Models.Generated.MatchedRecipient;
using RecipientResolutionResponse = Cheqi.Sdk.Models.Generated.RecipientResolutionResponse;

namespace Cheqi.Sdk.Tests.TestSupport;

internal static class TestData
{
    public static IdentificationDetails PairingCodeIdentification(string pairingCode = "PAIR-123")
    {
        var details = new IdentificationDetails(IdentificationDetails.PaymentTypeEnum.CARDPAYMENT);
        details.PairingCode = pairingCode;
        return details;
    }

    public static IdentificationDetails ReceiptIdIdentification(string cheqiReceiptId = "receipt-123", string? email = null)
    {
        var details = new IdentificationDetails(IdentificationDetails.PaymentTypeEnum.CARDPAYMENT);
        details.CheqiReceiptId = cheqiReceiptId;
        details.RecipientEmail = email;
        return details;
    }

    public static IdentificationDetails EmailIdentification(string email = "customer@example.com")
    {
        var details = new IdentificationDetails(IdentificationDetails.PaymentTypeEnum.CARDPAYMENT);
        details.RecipientEmail = email;
        return details;
    }

    public static RecipientResolutionResponse CustomerFoundResponse(string matchId = "match-123", List<MatchedRecipient>? recipients = null)
    {
        var response = new RecipientResolutionResponse();
        response.CustomerFound = true;
        response.MatchId = matchId;
        response.Recipients = recipients ?? new List<MatchedRecipient> { CreateRecipient() };
        return response;
    }

    public static MatchedRecipient CreateRecipient(string id = "recipient-1")
    {
        using var rsa = RSA.Create(2048);
        var publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());

        var recipient = new MatchedRecipient();
        recipient.Id = id;
        recipient.PublicKey = publicKey;
        recipient.KeyAlgorithm = "RSA";
        return recipient;
    }

    public static ReceiptTemplateRequest CreateReceiptTemplateRequest(string documentNumber = "INV-001") =>
        ReceiptTemplateRequest.Builder()
            .DocumentNumber(documentNumber)
            .IssueDate(DateTimeOffset.Parse("2026-03-15T10:00:00Z"))
            .Currency("EUR")
            .ReceiptSubtotal(100m)
            .TotalBeforeTax(100m)
            .TotalTaxAmount(21m)
            .TotalAmount(121m)
            .AddProduct(Product.Builder()
                .Identifier("SKU-1")
                .BrandName("Cheqi")
                .Name("Subscription")
                .Quantity(1)
                .BaseQuantity(1)
                .UnitCode(UnitCode.ONE)
                .UnitPrice(100m)
                .Subtotal(100m)
                .Total(121m)
                .AddTax(21.0, "VAT", 21m)
                .Build())
            .AddTax(Tax.Builder()
                .Rate(21.0)
                .Type("VAT")
                .Amount(21m)
                .Build())
            .Build();

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
