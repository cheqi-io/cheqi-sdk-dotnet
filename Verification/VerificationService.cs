using Cheqi.Sdk.Utils;

namespace Cheqi.Sdk.Verification;

/// <summary>
/// Receipt integrity verification through canonicalization and hashing.
/// </summary>
public sealed class VerificationService
{
    private readonly RFC8785Canonicalizer _jsonCanonicalizer;
    private readonly XmlCanonicalService _xmlCanonicalizer;

    public VerificationService()
    {
        _jsonCanonicalizer = new RFC8785Canonicalizer();
        _xmlCanonicalizer = new XmlCanonicalService();
    }

    public string CalculateCheqiReceiptHash(string cheqiReceiptJson)
    {
        return _jsonCanonicalizer.CalculateHash(cheqiReceiptJson);
    }

    public string CalculateCheqiReceiptHash(object cheqiReceipt)
    {
        return _jsonCanonicalizer.CalculateHash(cheqiReceipt);
    }

    public string CanonicalizeCheqiReceipt(string cheqiReceiptJson)
    {
        return _jsonCanonicalizer.Canonicalize(cheqiReceiptJson);
    }

    public string CanonicalizeCheqiReceipt(object cheqiReceipt)
    {
        return _jsonCanonicalizer.Canonicalize(cheqiReceipt);
    }

    public string CalculateUblHash(string ublXml)
    {
        return _xmlCanonicalizer.CalculateHash(ublXml);
    }

    public string CanonicalizeUbl(string ublXml)
    {
        return _xmlCanonicalizer.Canonicalize(ublXml);
    }
}
