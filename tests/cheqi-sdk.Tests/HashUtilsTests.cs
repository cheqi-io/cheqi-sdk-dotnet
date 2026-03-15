using Cheqi.Sdk.Utils;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class HashUtilsTests
{
    [Fact]
    public void Sha256Hex_String_KnownVectorMatchesExpected()
    {
        Assert.Equal(
            "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad",
            HashUtils.Sha256Hex("abc"));
    }

    [Fact]
    public void Sha256Hex_String_NullThrows()
    {
        Assert.Throws<ArgumentNullException>(() => HashUtils.Sha256Hex((string)null!));
    }

    [Fact]
    public void Sha256Hex_Bytes_ReturnsLowercase64CharHex()
    {
        var hash = HashUtils.Sha256Hex(new byte[] { 1, 2, 3 });

        Assert.Equal(64, hash.Length);
        Assert.Matches("^[0-9a-f]{64}$", hash);
    }

    [Fact]
    public void Sha256Hex_Bytes_IsDeterministic()
    {
        var input = new byte[] { 4, 5, 6 };

        Assert.Equal(HashUtils.Sha256Hex(input), HashUtils.Sha256Hex(input));
    }
}
