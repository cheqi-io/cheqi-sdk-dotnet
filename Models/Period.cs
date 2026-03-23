namespace Cheqi.Sdk.Models;

/// <summary>
/// Represents a billing or service period.
/// Extends the generated base with a builder pattern and DateTimeOffset convenience methods.
///
/// <para><b>Example:</b></para>
/// <code>
/// var period = Period.Builder()
///     .StartDate(new DateTime(2024, 1, 1))
///     .EndDate(new DateTime(2024, 1, 31))
///     .Description("January 2024")
///     .Build();
/// </code>
/// </summary>
public sealed class Period : Cheqi.Sdk.Models.Generated.Period
{
    public Period() : base() { }

    // ===== BUILDER =====

    public static PeriodBuilder Builder() => new PeriodBuilder();

    public sealed class PeriodBuilder
    {
        private DateTimeOffset? _startDate;
        private DateTimeOffset? _endDate;
        private string? _description;

        internal PeriodBuilder() { }

        public PeriodBuilder From(Generated.Period other)
        {
            if (other.StartDate != null)
                _startDate = DateTimeOffset.Parse(other.StartDate);
            if (other.EndDate != null)
                _endDate = DateTimeOffset.Parse(other.EndDate);
            _description = other.Description;
            return this;
        }

        public PeriodBuilder StartDate(DateTimeOffset startDate)
        {
            _startDate = startDate;
            return this;
        }

        public PeriodBuilder StartDate(DateTime startDate)
        {
            _startDate = new DateTimeOffset(startDate, TimeSpan.Zero);
            return this;
        }

        public PeriodBuilder EndDate(DateTimeOffset endDate)
        {
            _endDate = endDate;
            return this;
        }

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

            var p = new Period();
            p.StartDate = _startDate.Value.ToString("o");
            p.EndDate = _endDate.Value.ToString("o");
            p.Description = _description;
            return p;
        }
    }
}
