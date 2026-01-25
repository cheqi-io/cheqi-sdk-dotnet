using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Discount applied to a product line.
/// You can specify either:
/// - a fixed amount (e.g. 5.00), or
/// - a percentage (e.g. 10.0 for 10%)
/// The POS/backend is responsible for the actual calculation.
/// </summary>
public sealed class Discount
{
    /// <summary>
    /// Fixed discount amount for this line (e.g. 5.00 means 5 currency units off).
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal? amount { get; }

    /// <summary>
    /// Percentage discount (e.g. 10.0 means 10% off).
    /// </summary>
    [JsonPropertyName("percentage")]
    public double? percentage { get; }

    /// <summary>
    /// Human-readable label, e.g. "Black Friday", "Coupon", "Loyalty".
    /// </summary>
    [JsonPropertyName("label")]
    public string? label { get; }

    // ===== CONSTRUCTOR =====

    private Discount(decimal? amount, double? percentage, string? label)
    {
        this.amount = amount;
        this.percentage = percentage;
        this.label = label;
    }

    // ===== FACTORY METHODS =====

    /// <summary>
    /// Creates a discount with a fixed amount.
    /// </summary>
    /// <param name="amount">The discount amount</param>
    /// <param name="label">The discount label/description</param>
    /// <returns>A new Discount instance</returns>
    public static Discount Of(decimal amount, string? label)
    {
        return new Discount(amount, null, label);
    }

    /// <summary>
    /// Creates a discount with both amount and percentage.
    /// </summary>
    /// <param name="amount">The discount amount</param>
    /// <param name="percentage">The discount percentage</param>
    /// <param name="label">The discount label/description</param>
    /// <returns>A new Discount instance</returns>
    public static Discount Of(decimal amount, double percentage, string? label)
    {
        return new Discount(amount, percentage, label);
    }

    // ===== BUILDER =====

    public static DiscountBuilder Builder() => new DiscountBuilder();

    public sealed class DiscountBuilder
    {
        private decimal? _amount;
        private double? _percentage;
        private string? _label;

        internal DiscountBuilder() { }

        public DiscountBuilder From(Discount other)
        {
            _amount = other.amount;
            _percentage = other.percentage;
            _label = other.label;
            return this;
        }

        public DiscountBuilder Amount(decimal amount)
        {
            _amount = amount;
            return this;
        }

        public DiscountBuilder Percentage(double percentage)
        {
            _percentage = percentage;
            return this;
        }

        public DiscountBuilder Label(string? label)
        {
            _label = label;
            return this;
        }

        public Discount Build()
        {
            if (_amount != null && _percentage != null)
            {
                throw new InvalidOperationException(
                    "Discount: only one of amount or percentage should be set");
            }
            if (_amount == null && _percentage == null)
            {
                throw new InvalidOperationException(
                    "Discount: one of amount or percentage must be set");
            }
            return new Discount(_amount, _percentage, _label);
        }
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not Discount other) return false;
        return amount == other.amount
            && percentage == other.percentage
            && label == other.label;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(amount, percentage, label);
    }

    public override string ToString()
    {
        return $"Discount{{amount={amount}, percentage={percentage}, label='{label}'}}";
    }
}
