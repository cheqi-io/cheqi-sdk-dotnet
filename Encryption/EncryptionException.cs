using Cheqi.Sdk.Exceptions;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Exception thrown when encryption or decryption operations fail.
/// </summary>
public class EncryptionException : CheqiSDKException
{
    public EncryptionException(string message) : base(message)
    {
    }

    public EncryptionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
