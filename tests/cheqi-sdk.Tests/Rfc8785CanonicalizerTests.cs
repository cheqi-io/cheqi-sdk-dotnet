using Cheqi.Sdk.Utils;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class Rfc8785CanonicalizerTests
{
    private readonly RFC8785Canonicalizer _canonicalizer = new();

    [Fact]
    public void Canonicalize_SortsObjectKeys()
    {
        var canonical = _canonicalizer.Canonicalize("{\"z\":1,\"a\":2}");

        Assert.Equal("{\"a\":2,\"z\":1}", canonical);
    }

    [Fact]
    public void CalculateHash_IgnoresFormattingAndKeyOrder()
    {
        var compact = "{\"a\":1,\"b\":2}";
        var formatted = "{\n  \"b\": 2,\n  \"a\": 1\n}";

        Assert.Equal(_canonicalizer.CalculateHash(compact), _canonicalizer.CalculateHash(formatted));
    }

    [Fact]
    public void Canonicalize_EmptyInputThrows()
    {
        Assert.Throws<ArgumentException>(() => _canonicalizer.Canonicalize(""));
    }

    [Fact]
    public void Canonicalize_ObjectProducesStableJson()
    {
        var canonical = _canonicalizer.Canonicalize(new { b = 2, a = 1 });

        Assert.Equal("{\"a\":1,\"b\":2}", canonical);
    }
    // Fixtures from Java BigDecimal.valueOf(Double.parseDouble(token)).toPlainString().
    [Theory]
    [InlineData("0.1", "0.1")]
    [InlineData("2.10", "2.1")]
    [InlineData("1.0", "1.0")]
    [InlineData("-0.0", "0.0")]
    [InlineData("1e-7", "0.00000010")]
    [InlineData("0.001", "0.001")]
    [InlineData("1000000.0", "1000000.0")]
    [InlineData("1e21", "1000000000000000000000")]
    [InlineData("9007199254740993", "9007199254740993")]
    [InlineData("333333333.33333329", "333333333.3333333")]
    public void NumericTokensMatchJavaCanonicalization(string input, string expected)
        => Assert.Equal(expected, _canonicalizer.Canonicalize(input));

    [Fact]
    public void ModelAndWireJsonProduceTheSameHash()
    {
        var payload = TestSupport.SdkHarness.Payload();
        payload.TotalAmount = 12.1000m;
        var json = System.Text.Json.JsonSerializer.Serialize(payload, Serialization.CheqiJson.CreateOptions());
        Assert.Equal(_canonicalizer.CalculateHash(json), _canonicalizer.CalculateHash(payload));
        Assert.Contains("\"totalAmount\":12.1", json);
        Assert.Contains("\"issueDate\":\"2026-09-29T10:00:00Z\"", json);
        Assert.DoesNotContain("Option", _canonicalizer.Canonicalize(payload));
    }

    [Fact]
    public void FractionalDecimalsKeepTheirMagnitudeUnderAnyCulture()
    {
        var options = Serialization.CheqiJson.CreateOptions();
        var original = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("nl-NL");
            Assert.Equal("0.000000000125", System.Text.Json.JsonSerializer.Serialize(0.000000000125m, options));
            Assert.Equal("0.0000001", System.Text.Json.JsonSerializer.Serialize(0.0000001d, options));
        }
        finally { System.Globalization.CultureInfo.CurrentCulture = original; }
    }
}
