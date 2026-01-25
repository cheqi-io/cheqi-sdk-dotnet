using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Tax applied to a product line or receipt.
/// 
/// <para>Typical examples:</para>
/// <list type="bullet">
///   <item>VAT 21%</item>
///   <item>GST 10%</item>
///   <item>Sales Tax 8.25%</item>
/// </list>
/// 
/// <para><b>Example:</b></para>
/// <code>
/// var tax = Tax.Builder()
///     .Rate(19.0)                    // 19% tax rate
///     .Type("VAT")
///     .TaxableAmount(8500.00m)       // €8500 taxable amount
///     .Amount(1615.00m)              // €1615 tax (8500 × 0.19)
///     .Label("VAT 19%")
///     .Build();
/// </code>
/// </summary>
public sealed class Tax
{
    /// <summary>
    /// The tax rate as a percentage (e.g. 21.0 for 21%).
    /// </summary>
    [JsonPropertyName("rate")]
    public double? Rate { get; }

    /// <summary>
    /// The type of tax, e.g. "VAT", "GST", "Sales Tax".
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; }

    /// <summary>
    /// The taxable amount (base amount before tax).
    /// This is the amount that the tax rate is applied to.
    /// 
    /// <para>Example: €8500 taxable amount × 19% = €1615 tax</para>
    /// </summary>
    [JsonPropertyName("taxableAmount")]
    public decimal? TaxableAmount { get; }

    /// <summary>
    /// The calculated tax amount.
    /// Should equal: taxableAmount × (rate / 100)
    /// 
    /// <para>Example: €8500 × 0.19 = €1615</para>
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal? Amount { get; }

    /// <summary>
    /// Optional label, e.g. "VAT 21%" or "Reduced Rate".
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; }

    // ===== CONSTRUCTOR =====

    private Tax(double? rate, string? type, decimal? taxableAmount, decimal? amount, string? label)
    {
        Rate = rate;
        Type = type;
        TaxableAmount = taxableAmount;
        Amount = amount;
        Label = label;
    }

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

        public TaxBuilder From(Tax other)
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

        /// <summary>
        /// Sets the taxable amount (base amount before tax).
        /// </summary>
        /// <param name="taxableAmount">The amount that tax is calculated on</param>
        public TaxBuilder TaxableAmount(decimal taxableAmount)
        {
            _taxableAmount = taxableAmount;
            return this;
        }

        /// <summary>
        /// Sets the taxable amount (base amount before tax).
        /// </summary>
        /// <param name="taxableAmount">The amount that tax is calculated on</param>
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

            return new Tax(_rate, _type, _taxableAmount, _amount, _label);
        }
    }

    // ===== EQUALITY =====

    public override bool Equals(object? obj)
    {
        if (obj is not Tax other) return false;
        return Rate == other.Rate
            && Type == other.Type
            && TaxableAmount == other.TaxableAmount
            && Amount == other.Amount
            && Label == other.Label;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Rate, Type, TaxableAmount, Amount, Label);
    }

    public override string ToString()
    {
        return $"Tax{{rate={Rate}, type='{Type}', taxableAmount={TaxableAmount}, amount={Amount}, label='{Label}'}}";
    }
}
