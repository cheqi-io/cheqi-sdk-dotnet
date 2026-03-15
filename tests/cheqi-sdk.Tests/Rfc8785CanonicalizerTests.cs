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
}
