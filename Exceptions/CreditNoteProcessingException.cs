namespace Cheqi.Sdk.Exceptions;

/// <summary>
/// Exception thrown when credit note processing fails.
/// </summary>
public class CreditNoteProcessingException : Exception
{
    public CreditNoteProcessingException(string message) : base(message)
    {
    }

    public CreditNoteProcessingException(string message, Exception cause) : base(message, cause)
    {
    }
}
