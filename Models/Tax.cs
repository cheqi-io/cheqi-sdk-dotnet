namespace Cheqi.Sdk.Models;

/// <summary>
/// Tax applied to a product line or receipt.
/// Extends the generated base with a builder pattern and convenience methods.
///
/// <para><b>Example:</b></para>
/// <code>
/// var tax = Tax.Builder()
///     .Rate(19.0)
///     .Type("VAT")
///     .TaxableAmount(8500.00m)
///     .Amount(1615.00m)
///     .Label("VAT 19%")
///     .Build();
/// </code>
/// </summary>
public sealed class Tax : Cheqi.Sdk.Models.Generated.Tax
{
    public Tax() : base(0, "", 0) { }

    // ===== BUILDER =====

    public static TaxBuilder Builder() => new TaxBuilder();

    public sealed class TaxBuilder
    {
        private double? _rate;
        private string? _type;
        private decimal? _taxableAmount;
        private decimal? _amount;
        private string? _label;

        internal TaxBuilder() { }

        public TaxBuilder From(Generated.Tax other)
        {
            _rate = other.Rate;
            _type = other.Type;
            _taxableAmount = other.TaxableAmount;
            _amount = other.Amount;
            _label = other.Label;
            return this;
        }

        public TaxBuilder Rate(double rate)
        {
            _rate = rate;
            return this;
        }

        public TaxBuilder Type(string type)
        {
            _type = type;
            return this;
        }

        public TaxBuilder TaxableAmount(decimal taxableAmount)
        {
            _taxableAmount = taxableAmount;
            return this;
        }

        public TaxBuilder TaxableAmount(string taxableAmount)
        {
            _taxableAmount = decimal.Parse(taxableAmount);
            return this;
        }

        public TaxBuilder Amount(decimal amount)
        {
            _amount = amount;
            return this;
        }

        public TaxBuilder Amount(string amount)
        {
            _amount = decimal.Parse(amount);
            return this;
        }

        public TaxBuilder Label(string label)
        {
            _label = label;
            return this;
        }

        public Tax Build()
        {
            if (_rate == null)
                throw new InvalidOperationException("Tax rate is required");

            if (string.IsNullOrWhiteSpace(_type))
                throw new InvalidOperationException("Tax type is required");

            var tax = new Tax();
            tax.Rate = _rate.Value;
            tax.Type = _type!;
            tax.TaxableAmount = _taxableAmount ?? 0;
            tax.Amount = _amount;
            tax.Label = _label;
            return tax;
        }
    }
}
