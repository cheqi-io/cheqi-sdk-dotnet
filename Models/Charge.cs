namespace Cheqi.Sdk.Models;

/// <summary>
/// Extra charge (fee) applied to a product line or receipt.
/// Extends the generated base with a builder pattern and factory methods.
/// Examples: "Shipping", "Service fee", "Bottle deposit".
/// </summary>
public sealed class Charge : Cheqi.Sdk.Models.Generated.Charge
{
    public Charge() : base(0) { }

    // ===== FACTORY METHODS =====

    public static Charge Of(decimal amount, string? label)
    {
        var c = new Charge();
        c.Amount = amount;
        c.Label = label;
        return c;
    }

    public static Charge Of(decimal amount, double percentage, string? label)
    {
        var c = new Charge();
        c.Amount = amount;
        c.Percentage = percentage;
        c.Label = label;
        return c;
    }

    // ===== BUILDER =====

    public static ChargeBuilder Builder() => new ChargeBuilder();

    public sealed class ChargeBuilder
    {
        private decimal? _amount;
        private double? _percentage;
        private string? _label;

        internal ChargeBuilder() { }

        public ChargeBuilder From(Generated.Charge other)
        {
            _amount = other.Amount;
            _percentage = other.Percentage;
            _label = other.Label;
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
            if (_amount == null)
                throw new InvalidOperationException("Charge: amount is required");

            var c = new Charge();
            c.Amount = _amount.Value;
            c.Percentage = _percentage;
            c.Label = _label;
            return c;
        }
    }
}
