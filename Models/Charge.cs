using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Extra charge (fee) applied to a product line.
/// Examples: "Shipping", "Service fee", "Bottle deposit".
/// </summary>
public sealed class Charge
{
    /// <summary>
    /// Fixed charge amount for this line (e.g. 5.00 means 5 currency units extra).
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal? amount { get; }

    /// <summary>
    /// Percentage charge (e.g. 3.0 means 3% fee).
    /// </summary>
    [JsonPropertyName("percentage")]
    public double? percentage { get; }

    /// <summary>
    /// Human-readable label, e.g. "Shipping", "Service fee".
    /// </summary>
    [JsonPropertyName("label")]
    public string? label { get; }

    // ===== CONSTRUCTOR =====

    private Charge(decimal? amount, double? percentage, string? label)
    {
        this.amount = amount;
        this.percentage = percentage;
        this.label = label;
    }

    // ===== FACTORY METHODS =====

    /// <summary>
    /// Creates a charge with a fixed amount.
    /// </summary>
    /// <param name="amount">The charge amount</param>
    /// <param name="label">The charge label/description</param>
    /// <returns>A new Charge instance</returns>
    public static Charge Of(decimal amount, string? label)
    {
        return new Charge(amount, null, label);
    }

    /// <summary>
    /// Creates a charge with both amount and percentage.
    /// </summary>
    /// <param name="amount">The charge amount</param>
    /// <param name="percentage">The charge percentage</param>
    /// <param name="label">The charge label/description</param>
    /// <returns>A new Charge instance</returns>
    public static Charge Of(decimal amount, double percentage, string? label)
    {
        return new Charge(amount, percentage, label);
    }

    // ===== BUILDER =====

    public static ChargeBuilder Builder() => new ChargeBuilder();

    public sealed class ChargeBuilder
    {
        private decimal? _amount;
        private double? _percentage;
        private string? _label;

        internal ChargeBuilder() { }

        public ChargeBuilder From(Charge other)
        {
            _amount = other.amount;
            _percentage = other.percentage;
            _label = other.label;
            return this;
        }

        public ChargeBuilder Amount(decimal amount)
        {
            _amount = amount;
            return this;
        }

        public ChargeBuilder Percentage(double percentage)
        {
            _percentage = percentage;
            return this;
        }

        public ChargeBuilder Label(string? label)
        {
            _label = label;
            return this;
        }

        public Charge Build()
        {
            if (_amount != null && _percentage != null)
            {
                throw new InvalidOperationException(
                    "Charge: only one of amount or percentage should be set");
            }
            if (_amount == null && _percentage == null)
            {
                throw new InvalidOperationException(
                    "Charge: one of amount or percentage must be set");
            }
            return new Charge(_amount, _percentage, _label);
        }
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not Charge other) return false;
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
        return $"Charge{{amount={amount}, percentage={percentage}, label='{label}'}}";
    }
}
