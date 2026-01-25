namespace Cheqi.Sdk.Receipt;

public class ValidationResult
{
    public bool Valid { get; }
    public IReadOnlyList<string> Errors { get; }

    private ValidationResult(bool valid, IReadOnlyList<string> errors)
    {
        Valid = valid;
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>ValidationResult indicating validation passed with no errors</returns>
    public static ValidationResult ValidResult()
    {
        return new ValidationResult(true, Array.Empty<string>());
    }

    /// <summary>
    /// Creates a failed validation result with a single error message.
    /// </summary>
    /// <param name="error">Error message describing the validation failure</param>
    /// <returns>ValidationResult indicating validation failed</returns>
    public static ValidationResult Invalid(string error)
    {
        if (error == null)
            throw new ArgumentNullException(nameof(error));
        
        return new ValidationResult(false, new[] { error });
    }

    /// <summary>
    /// Creates a failed validation result with multiple error messages.
    /// </summary>
    /// <param name="errors">List of error messages describing validation failures</param>
    /// <returns>ValidationResult indicating validation failed</returns>
    public static ValidationResult Invalid(List<string> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors));
        if (errors.Count == 0)
            throw new ArgumentException("errors list cannot be empty for invalid result", nameof(errors));
        
        return new ValidationResult(false, errors.AsReadOnly());
    }

    /// <summary>
    /// Checks if the validation passed.
    /// </summary>
    /// <returns>true if validation passed, false otherwise</returns>
    public bool IsValid() => Valid;

    /// <summary>
    /// Gets all validation errors as a single comma-separated string.
    /// </summary>
    /// <returns>Comma-separated string of all error messages, or empty string if valid</returns>
    public string GetErrorMessage()
    {
        return string.Join(", ", Errors);
    }

    public override bool Equals(object? obj)
    {
        if (obj is ValidationResult other)
        {
            return Valid == other.Valid && Errors.SequenceEqual(other.Errors);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Valid, Errors);
    }

    public override string ToString()
    {
        return $"ValidationResult{{valid={Valid}, errors=[{string.Join(", ", Errors)}]}}";
    }
}
