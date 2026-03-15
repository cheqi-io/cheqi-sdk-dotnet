using Cheqi.Sdk.Verification;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class VerificationServiceTests
{
    private readonly VerificationService _service = new();

    [Fact]
    public void CalculateUblXmlHash_IgnoresFormattingWhitespaceBetweenElements()
    {
        const string compactXml = "<Invoice><AccountingCustomerParty><Party><Name>Cheqi</Name></Party></AccountingCustomerParty></Invoice>";
        var formattedXml = "<Invoice>\n"
            + "  <AccountingCustomerParty>\n"
            + "    <Party>\n"
            + "      <Name>Cheqi</Name>\n"
            + "    </Party>\n"
            + "  </AccountingCustomerParty>\n"
            + "</Invoice>";

        Assert.Equal(_service.CalculateUblXmlHash(compactXml), _service.CalculateUblXmlHash(formattedXml));
    }

    [Fact]
    public void CalculateUblXmlHash_PreservesMeaningfulTextWhitespace()
    {
        const string singleSpaced = "<Invoice><Name>Cheqi BV</Name></Invoice>";
        const string doubleSpaced = "<Invoice><Name>Cheqi  BV</Name></Invoice>";

        Assert.NotEqual(_service.CalculateUblXmlHash(singleSpaced), _service.CalculateUblXmlHash(doubleSpaced));
    }
}
