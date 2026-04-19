using Cheqi.Sdk.Decryption;
using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.CreditNote;

public class CreditNoteMergeService
{
    private const string CustomerPartyPlaceholder = "<cac:AccountingCustomerParty/>";
    private const string CustomerPartyClosingTag = "</cac:AccountingCustomerParty>";

    public DecryptedDeliveredCreditNote Merge(DecryptedDeliveredCreditNote decryptedCreditNote)
    {
        var contextEnvelope = decryptedCreditNote.CreditNoteContextEnvelope;
        if (contextEnvelope == null)
        {
            return decryptedCreditNote;
        }

        var creditNoteEnvelope = decryptedCreditNote.CreditNoteEnvelope;
        MergeXmlParty(creditNoteEnvelope, contextEnvelope);
        MergeXmlPaymentMeans(creditNoteEnvelope, contextEnvelope);
        MergeReceivingParty(creditNoteEnvelope, contextEnvelope);

        return new DecryptedDeliveredCreditNote(
            creditNoteEnvelope,
            contextEnvelope,
            decryptedCreditNote.CreditNoteContentJson,
            decryptedCreditNote.CreditNoteContextJson);
    }

    private static void MergeXmlParty(CreditNoteEnvelope creditNoteEnvelope, CreditNoteContextEnvelope contextEnvelope)
    {
        string? xmlReceivingParty = contextEnvelope.ReceivingParty?.XmlReceivingParty;
        if (string.IsNullOrWhiteSpace(xmlReceivingParty) || string.IsNullOrWhiteSpace(creditNoteEnvelope.UblCreditNote))
        {
            return;
        }

        if (creditNoteEnvelope.UblCreditNote.Contains(CustomerPartyPlaceholder, StringComparison.Ordinal))
        {
            creditNoteEnvelope.UblCreditNote = creditNoteEnvelope.UblCreditNote.Replace(
                CustomerPartyPlaceholder,
                xmlReceivingParty,
                StringComparison.Ordinal);
        }
    }

    private static void MergeXmlPaymentMeans(CreditNoteEnvelope creditNoteEnvelope, CreditNoteContextEnvelope contextEnvelope)
    {
        string? xmlPaymentMeans = contextEnvelope.PaymentMeans?.XmlPaymentMeans;
        string? xml = creditNoteEnvelope.UblCreditNote;
        if (string.IsNullOrWhiteSpace(xmlPaymentMeans) || string.IsNullOrWhiteSpace(xml))
        {
            return;
        }

        if (xml.Contains(CustomerPartyClosingTag, StringComparison.Ordinal))
        {
            creditNoteEnvelope.UblCreditNote = xml.Replace(
                CustomerPartyClosingTag,
                CustomerPartyClosingTag + xmlPaymentMeans,
                StringComparison.Ordinal);
            return;
        }

        if (xml.Contains(CustomerPartyPlaceholder, StringComparison.Ordinal))
        {
            creditNoteEnvelope.UblCreditNote = xml.Replace(
                CustomerPartyPlaceholder,
                CustomerPartyPlaceholder + xmlPaymentMeans,
                StringComparison.Ordinal);
        }
    }

    private static void MergeReceivingParty(CreditNoteEnvelope creditNoteEnvelope, CreditNoteContextEnvelope contextEnvelope)
    {
        if (contextEnvelope.ReceivingParty?.ReceivingParty == null || creditNoteEnvelope.CheqiCreditNote == null)
        {
            return;
        }

        creditNoteEnvelope.CheqiCreditNote.ReceivingParty = contextEnvelope.ReceivingParty.ReceivingParty;
    }
}
