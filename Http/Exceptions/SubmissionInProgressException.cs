namespace Cheqi.Sdk.Http.Exceptions;

/// <summary>The original match is processing. Resume later using its opaque ID.</summary>
public sealed class SubmissionInProgressException : CheqiApiException
{
    public override bool Retryable => true;
    public int? RetryAfterSeconds { get; }
    public SubmissionInProgressException(string message, string? correlationId, int? retryAfterSeconds)
        : base(message, 202, "SUBMISSION_IN_PROGRESS", correlationId) => RetryAfterSeconds = retryAfterSeconds;
}
