namespace Cheqi.Sdk.Exceptions;

public class ReceiptProcessingException : Exception
{
    public ReceiptProcessingException(string message) : base(message)
    {
    }

    public ReceiptProcessingException(string message, Exception cause) : base(message, cause)
    {
    }
}
