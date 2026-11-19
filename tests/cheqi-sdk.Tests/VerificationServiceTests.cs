using Cheqi.Sdk.Verification;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class VerificationServiceTests
{
    private readonly VerificationService _service = new();

    [Fact]
    public void VerificationAcceptsEquivalentDocumentsAndRejectsChangedContent()
    {
        var jsonHash = _service.CanonicalizeAndHashJson(new { b = 2, a = 1 });
        Assert.True(_service.VerifyJsonHash("{\"a\":1,\"b\":2}", jsonHash));
        Assert.False(_service.VerifyJsonHash("{\"a\":2,\"b\":2}", jsonHash));
        var xmlHash = _service.CanonicalizeAndHashXml("<Invoice><Total>10</Total></Invoice>");
        Assert.True(_service.VerifyXmlHash("<Invoice>\n  <Total>10</Total>\n</Invoice>", xmlHash));
        Assert.False(_service.VerifyXmlHash("<Invoice><Total>11</Total></Invoice>", xmlHash));
    }

    [Fact]
    public void CalculateUblHash_IgnoresFormattingWhitespaceBetweenElements()
    {
        const string compactXml = "<Invoice><AccountingCustomerParty><Party><Name>Cheqi</Name></Party></AccountingCustomerParty></Invoice>";
        var formattedXml = "<Invoice>\n"
            + "  <AccountingCustomerParty>\n"
            + "    <Party>\n"
            + "      <Name>Cheqi</Name>\n"
            + "    </Party>\n"
            + "  </AccountingCustomerParty>\n"
            + "</Invoice>";

        Assert.Equal(_service.CalculateUblHash(compactXml), _service.CalculateUblHash(formattedXml));
    }

    [Fact]
    public void CalculateUblHash_PreservesMeaningfulTextWhitespace()
    {
        const string singleSpaced = "<Invoice><Name>Cheqi BV</Name></Invoice>";
        const string doubleSpaced = "<Invoice><Name>Cheqi  BV</Name></Invoice>";

        Assert.NotEqual(_service.CalculateUblHash(singleSpaced), _service.CalculateUblHash(doubleSpaced));
    }
}
