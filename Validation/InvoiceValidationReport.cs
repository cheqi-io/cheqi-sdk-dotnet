using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Validation;

/// <summary>A failed receipt rule, its field path, and an explanation.</summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record InvoiceValidationIssue
{
    public required string RuleId { get; init; }
    public required string Path { get; init; }
    public required string Message { get; init; }
}

/// <summary>All receipt-payload findings. Final document validation remains in the wallet.</summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record InvoiceValidationReport
{
    public required string Profile { get; init; }
    public required bool Valid { get; init; }
    public required bool CompleteDocument { get; init; }
    public required IReadOnlyList<InvoiceValidationIssue> Issues { get; init; }
}
