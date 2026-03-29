using Cheqi.Sdk.Verification;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class VerificationServiceTests
{
    private readonly VerificationService _service = new();

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
