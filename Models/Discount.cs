namespace Cheqi.Sdk.Models;

/// <summary>
/// Discount applied to a product line or receipt.
/// Extends the generated base with a builder pattern and factory methods.
/// </summary>
public sealed class Discount : Cheqi.Sdk.Models.Generated.Discount
{
    public Discount() : base(0) { }

    // ===== FACTORY METHODS =====

    public static Discount Of(decimal amount, string? label)
    {
        var d = new Discount();
        d.Amount = amount;
        d.Label = label;
        return d;
    }

    public static Discount Of(decimal amount, double percentage, string? label)
    {
        var d = new Discount();
        d.Amount = amount;
        d.Percentage = percentage;
        d.Label = label;
        return d;
    }

    // ===== BUILDER =====

    public static DiscountBuilder Builder() => new DiscountBuilder();

    public sealed class DiscountBuilder
    {
        private decimal? _amount;
        private double? _percentage;
        private string? _label;

        internal DiscountBuilder() { }

        public DiscountBuilder From(Generated.Discount other)
        {
            _amount = other.Amount;
            _percentage = other.Percentage;
            _label = other.Label;
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
            if (_amount == null)
                throw new InvalidOperationException("Discount: amount is required");

            var d = new Discount();
            d.Amount = _amount.Value;
            d.Percentage = _percentage;
            d.Label = _label;
            return d;
        }
    }
}
