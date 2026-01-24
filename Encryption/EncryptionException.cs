using Cheqi.Sdk.Exceptions;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Exception thrown when encryption or decryption operations fail.
/// </summary>
public class EncryptionException : CheqiSdkException
{
    public EncryptionException(string message) : base(message)
    {
    }

    public EncryptionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when decryption operations fail.
/// </summary>
public class DecryptionException : EncryptionException
{
    public DecryptionException(string message) : base(message)
    {
    }

    public DecryptionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
