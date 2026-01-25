using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Represents a billing or service period.
/// 
/// <para>Use this to specify the time range covered by a receipt, such as:</para>
/// <list type="bullet">
///   <item>Subscription periods (e.g., monthly SaaS from Jan 1 - Jan 31)</item>
///   <item>Rental periods (e.g., car rental from Dec 1 - Dec 7)</item>
///   <item>Utility billing periods (e.g., electricity from Nov 1 - Nov 30)</item>
///   <item>Service contracts (e.g., maintenance from Q1 2024)</item>
/// </list>
/// 
/// <para><b>Easy Usage Examples:</b></para>
/// <code>
/// // Simple date-only period (uses DateTime - easiest!)
/// var monthlyPeriod = Period.Builder()
///     .StartDate(new DateTime(2024, 1, 1))
///     .EndDate(new DateTime(2024, 1, 31))
///     .Description("January 2024")
///     .Build();
/// 
/// // With specific times (uses DateTime)
/// var rentalPeriod = Period.Builder()
///     .StartDate(new DateTime(2024, 12, 1, 14, 0, 0))  // 2:00 PM
///     .EndDate(new DateTime(2024, 12, 1, 17, 0, 0))    // 5:00 PM
///     .Description("3-hour rental")
///     .Build();
/// 
/// // With timezone support (uses DateTimeOffset)
/// var eventPeriod = Period.Builder()
///     .StartDate(new DateTimeOffset(2024, 6, 15, 19, 0, 0, TimeSpan.FromHours(2)))
///     .EndDate(new DateTimeOffset(2024, 6, 15, 22, 0, 0, TimeSpan.FromHours(2)))
///     .Description("Concert")
///     .Build();
/// </code>
/// 
/// <para><b>Mandatory Fields:</b></para>
/// <list type="bullet">
///   <item><b>startDate</b>: The start date of the period</item>
///   <item><b>endDate</b>: The end date of the period</item>
/// </list>
/// 
/// <para><b>Optional Fields:</b></para>
/// <list type="bullet">
///   <item><b>description</b>: Human-readable description of the period</item>
/// </list>
/// </summary>
public sealed class Period
{
    // ===== MANDATORY FIELDS =====

    /// <summary>
    /// The start date/time of the period.
    /// Uses ISO-8601 format with timezone (e.g., "2024-01-01T00:00:00Z").
    /// For date-only periods, use midnight UTC (e.g., "2024-01-01T00:00:00Z").
    /// </summary>
    [JsonPropertyName("startDate")]
    public DateTimeOffset startDate { get; }

    /// <summary>
    /// The end date/time of the period.
    /// Uses ISO-8601 format with timezone (e.g., "2024-01-31T23:59:59Z").
    /// For date-only periods, use end of day UTC (e.g., "2024-01-31T23:59:59Z").
    /// </summary>
    [JsonPropertyName("endDate")]
    public DateTimeOffset endDate { get; }

    // ===== OPTIONAL FIELDS =====

    /// <summary>
    /// Optional human-readable description of the period.
    /// Examples: "Monthly subscription - January 2024", "Q1 2024 service period"
    /// </summary>
    [JsonPropertyName("description")]
    public string? description { get; }

    // ===== CONSTRUCTOR =====

    private Period(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        string? description)
    {
        this.startDate = startDate;
        this.endDate = endDate;
        this.description = description;
    }

    // ===== BUILDER =====

    public static PeriodBuilder Builder() => new PeriodBuilder();

    public sealed class PeriodBuilder
    {
        private DateTimeOffset? _startDate;
        private DateTimeOffset? _endDate;
        private string? _description;

        internal PeriodBuilder() { }

        public PeriodBuilder From(Period other)
        {
            _startDate = other.startDate;
            _endDate = other.endDate;
            _description = other.description;
            return this;
        }

        /// <summary>
        /// Sets the start date/time of the period.
        /// </summary>
        public PeriodBuilder StartDate(DateTimeOffset startDate)
        {
            _startDate = startDate;
            return this;
        }

        /// <summary>
        /// Sets the start date (at midnight UTC).
        /// Convenient method for date-only periods.
        /// </summary>
        public PeriodBuilder StartDate(DateTime startDate)
        {
            _startDate = new DateTimeOffset(startDate, TimeSpan.Zero);
            return this;
        }

        /// <summary>
        /// Sets the end date/time of the period.
        /// </summary>
        public PeriodBuilder EndDate(DateTimeOffset endDate)
        {
            _endDate = endDate;
            return this;
        }

        /// <summary>
        /// Sets the end date (at end of day - 23:59:59 UTC).
        /// Convenient method for date-only periods.
        /// </summary>
        public PeriodBuilder EndDate(DateTime endDate)
        {
            var endOfDay = endDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            _endDate = new DateTimeOffset(endOfDay, TimeSpan.Zero);
            return this;
        }

        public PeriodBuilder Description(string? description)
        {
            _description = description;
            return this;
        }

        public Period Build()
        {
            if (_startDate == null)
                throw new InvalidOperationException("StartDate is required");
            if (_endDate == null)
                throw new InvalidOperationException("EndDate is required");

            return new Period(_startDate.Value, _endDate.Value, _description);
        }
    }

    // ===== VALIDATION METHODS =====

    /// <summary>
    /// Validates this period.
    /// </summary>
    /// <returns>true if both start and end timestamps are present and start is before or equal to end</returns>
    public bool IsValid() => GetValidationErrors().Count == 0;

    /// <summary>
    /// Gets detailed validation errors for this period.
    /// </summary>
    /// <returns>List of validation error messages, empty if valid</returns>
    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (startDate == default)
            errors.Add("Start date/time is required");

        if (endDate == default)
            errors.Add("End date/time is required");

        if (startDate != default && endDate != default && startDate > endDate)
            errors.Add("Start date/time must be before or equal to end date/time");

        return errors;
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not Period other) return false;
        return startDate == other.startDate
            && endDate == other.endDate
            && description == other.description;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(startDate, endDate, description);
    }

    public override string ToString()
    {
        return $"Period{{startDate={startDate:O}, endDate={endDate:O}, description={description}}}";
    }
}
