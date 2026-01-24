namespace Cheqi.Sdk.Exceptions;

/// <summary>
/// Base exception for all Cheqi SDK errors.
/// </summary>
public class CheqiSdkException : Exception
{
    /// <summary>
    /// Error code identifying the type of error.
    /// </summary>
    public string? ErrorCode { get; }

    public CheqiSdkException(string message) : base(message)
    {
    }

    public CheqiSdkException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public CheqiSdkException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public CheqiSdkException(string message, string errorCode, Exception innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Exception thrown when receipt processing fails.
/// </summary>
public class ReceiptProcessingException : CheqiSdkException
{
    public ReceiptProcessingException(string message) : base(message)
    {
    }

    public ReceiptProcessingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when credit note processing fails.
/// </summary>
public class CreditNoteProcessingException : CheqiSdkException
{
    public CreditNoteProcessingException(string message) : base(message)
    {
    }

    public CreditNoteProcessingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when customer matching fails.
/// </summary>
public class MatchingException : CheqiSdkException
{
    public MatchingException(string message) : base(message)
    {
    }

    public MatchingException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
