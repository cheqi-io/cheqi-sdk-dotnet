using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Simple product line item used in Cheqi receipts.
/// This is intentionally "dumb":
/// - No UBL/PEPPOL types
/// - POS/backend just fills in what it already knows
/// Cheqi backend handles UBL mapping and validation.
///
/// <para><b>Typical usage:</b></para>
/// <code>
/// // Simple product (1 laptop)
/// var laptop = Product.Builder()
///     .Name("Laptop 13\"")
///     .Identifier("LAP-001")
///     .Quantity(1.0)
///     .BaseQuantity(1.0)        // Always required (1.0 for simple items)
///     .UnitCode(UnitCode.ONE)
///     .UnitPrice(1000.00m)
///     .Subtotal(1000.00m)
///     .Total(1210.00m)
///     .AddTax(21.0, "VAT", 210.00m)
///     .Build();
///
/// // Pre-packaged product (10 packages × 500g cheese)
/// var cheese = Product.Builder()
///     .Name("Gouda Cheese")
///     .Identifier("CHEESE-500G")
///     .Quantity(10.0)           // 10 packages
///     .BaseQuantity(500.0)      // 500g per package (total: 5000g)
///     .UnitCode(UnitCode.GRAM)
///     .UnitPrice(4.99m)         // Price per package
///     .Subtotal(49.90m)
///     .Total(60.38m)
///     .AddTax(21.0, "VAT", 10.48m)
///     .Build();
///
/// // Weight-based product (2.5kg apples)
/// var apples = Product.Builder()
///     .Name("Organic Apples")
///     .Identifier("APPLE-ORG")
///     .Quantity(2.5)
///     .BaseQuantity(1.0)
///     .UnitCode(UnitCode.KILOGRAM)
///     .UnitPrice(3.99m)        // Price per kg
///     .Subtotal(9.98m)
///     .Total(12.08m)
///     .AddTax(21.0, "VAT", 2.10m)
///     .Build();
///
/// // Time-based product (monthly subscription)
/// var subscription = Product.Builder()
///     .Name("Premium Subscription")
///     .Identifier("SUB-PREMIUM-M")
///     .Quantity(1.0)
///     .BaseQuantity(1.0)
///     .UnitCode(UnitCode.MONTH)
///     .UnitPrice(29.99m)
///     .Subtotal(29.99m)
///     .Total(36.29m)
///     .AddTax(21.0, "VAT", 6.30m)
///     .Period(Period.Builder()
///         .StartDate(new DateTime(2024, 1, 1))
///         .EndDate(new DateTime(2024, 1, 31))
///         .Description("January 2024")
///         .Build())
///     .Build();
/// </code>
/// </summary>
public sealed class Product
{
    // ===== BASIC INFO =====

    /// <summary>
    /// Human-readable product name shown on the receipt.
    /// </summary>
    [JsonPropertyName("name")]
    public string name { get; }

    /// <summary>
    /// Article identifier, for example article number (merchant-defined).
    /// </summary>
    [JsonPropertyName("identifier")]
    public string? identifier { get; }

    /// <summary>
    /// Optional longer description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? description { get; }

    /// <summary>
    /// Optional brand name.
    /// </summary>
    [JsonPropertyName("brand")]
    public string? brand { get; }

    // ===== QUANTITY & PRICE =====

    /// <summary>
    /// Quantity of this item (e.g., 1, 2.5, etc.).
    /// </summary>
    [JsonPropertyName("quantity")]
    public double? quantity { get; }

    /// <summary>
    /// Base quantity per unit (mandatory).
    /// 
    /// <para>For pre-packaged products sold in fixed quantities:</para>
    /// <list type="bullet">
    ///   <item>10 packages × 500g cheese → quantity=10, baseQuantity=500, unitCode=GRAM</item>
    ///   <item>6 bottles × 0.33L water → quantity=6, baseQuantity=0.33, unitCode=LITER</item>
    ///   <item>3 boxes × 12 eggs → quantity=3, baseQuantity=12, unitCode=ONE</item>
    /// </list>
    /// 
    /// <para>For simple items, set baseQuantity to 1.0:</para>
    /// <list type="bullet">
    ///   <item>2.5 kg apples → quantity=2.5, baseQuantity=1.0, unitCode=KILOGRAM</item>
    ///   <item>1 laptop → quantity=1, baseQuantity=1.0, unitCode=ONE</item>
    /// </list>
    /// 
    /// <para><b>Total quantity = quantity × baseQuantity</b></para>
    /// </summary>
    [JsonPropertyName("baseQuantity")]
    public double? baseQuantity { get; }

    /// <summary>
    /// Unit of measure code (mandatory).
    /// Uses UN/ECE Recommendation 20 standard codes.
    /// Examples: ONE (C62), KILOGRAM (KGM), LITER (LTR), HOUR (HUR)
    /// </summary>
    [JsonPropertyName("unitCode")]
    public UnitCode? unitCode { get; }

    /// <summary>
    /// Unit price (net, before tax) as seen in the POS.
    /// </summary>
    [JsonPropertyName("unitPrice")]
    public decimal? unitPrice { get; }

    // ===== DISCOUNTS & CHARGES =====

    /// <summary>
    /// Discounts applied to this line (POS-calculated).
    /// </summary>
    [JsonPropertyName("discounts")]
    public IReadOnlyList<Discount> discounts { get; }

    /// <summary>
    /// Extra charges applied to this line (POS-calculated).
    /// </summary>
    [JsonPropertyName("charges")]
    public IReadOnlyList<Charge> charges { get; }

    // ===== TAXES =====

    /// <summary>
    /// Taxes applied to this line.
    /// For many countries there will be exactly one tax (e.g. 21% VAT),
    /// but multiple entries are allowed for flexibility.
    /// </summary>
    [JsonPropertyName("taxes")]
    public IReadOnlyList<Tax> taxes { get; }

    // ===== TOTALS (FROM POS) =====

    /// <summary>
    /// Line subtotal, before tax.
    /// Typically: quantity * unitPrice - discounts + charges (net).
    /// </summary>
    [JsonPropertyName("subtotal")]
    public decimal? subtotal { get; }

    /// <summary>
    /// Line total, after tax.
    /// Typically: subtotal + tax amounts (gross).
    /// </summary>
    [JsonPropertyName("total")]
    public decimal? total { get; }

    // ===== OPTIONAL FIELDS =====

    /// <summary>
    /// Optional period for time-based products.
    /// 
    /// <para>Use this for products that represent services over a time range:</para>
    /// <list type="bullet">
    ///   <item>Subscriptions (e.g., monthly SaaS from Jan 1 - Jan 31)</item>
    ///   <item>Rentals (e.g., car rental from Dec 1 - Dec 7)</item>
    ///   <item>Utilities (e.g., electricity usage from Nov 1 - Nov 30)</item>
    ///   <item>Service contracts (e.g., maintenance from Q1 2024)</item>
    /// </list>
    /// 
    /// <para>Leave empty for one-time purchases or instant transactions.</para>
    /// </summary>
    [JsonPropertyName("period")]
    public Period? period { get; }

    // ===== CONSTRUCTOR =====

    private Product(
        string name,
        string? identifier,
        string? description,
        string? brand,
        double? quantity,
        double? baseQuantity,
        UnitCode? unitCode,
        decimal? unitPrice,
        List<Discount>? discounts,
        List<Charge>? charges,
        List<Tax>? taxes,
        decimal? subtotal,
        decimal? total,
        Period? period)
    {
        this.name = name;
        this.identifier = identifier;
        this.description = description;
        this.brand = brand;
        this.quantity = quantity;
        this.baseQuantity = baseQuantity;
        this.unitCode = unitCode;
        this.unitPrice = unitPrice;
        // Defensive copies: keep internal lists immutable
        this.discounts = discounts?.AsReadOnly() ?? new List<Discount>().AsReadOnly();
        this.charges = charges?.AsReadOnly() ?? new List<Charge>().AsReadOnly();
        this.taxes = taxes?.AsReadOnly() ?? new List<Tax>().AsReadOnly();
        this.subtotal = subtotal;
        this.total = total;
        this.period = period;
    }

    // ===== BUILDER =====

    public static ProductBuilder Builder() => new ProductBuilder();

    public sealed class ProductBuilder
    {
        private string? _name;
        private string? _identifier;
        private string? _description;
        private string? _brand;
        private double? _quantity;
        private double? _baseQuantity;
        private UnitCode? _unitCode;
        private decimal? _unitPrice;
        private List<Discount> _discounts = new();
        private List<Charge> _charges = new();
        private List<Tax> _taxes = new();
        private decimal? _subtotal;
        private decimal? _total;
        private Period? _period;

        internal ProductBuilder() { }

        public ProductBuilder From(Product other)
        {
            _name = other.name;
            _identifier = other.identifier;
            _description = other.description;
            _brand = other.brand;
            _quantity = other.quantity;
            _baseQuantity = other.baseQuantity;
            _unitCode = other.unitCode;
            _unitPrice = other.unitPrice;
            _discounts = new List<Discount>(other.discounts);
            _charges = new List<Charge>(other.charges);
            _taxes = new List<Tax>(other.taxes);
            _subtotal = other.subtotal;
            _total = other.total;
            _period = other.period;
            return this;
        }

        public ProductBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public ProductBuilder Identifier(string? identifier)
        {
            _identifier = identifier;
            return this;
        }

        public ProductBuilder Description(string? description)
        {
            _description = description;
            return this;
        }

        public ProductBuilder Brand(string? brand)
        {
            _brand = brand;
            return this;
        }

        public ProductBuilder Quantity(double quantity)
        {
            _quantity = quantity;
            return this;
        }

        /// <summary>
        /// Sets the base quantity per unit (mandatory).
        /// Use 1.0 for simple items, or the package size for pre-packaged items.
        /// </summary>
        public ProductBuilder BaseQuantity(double baseQuantity)
        {
            _baseQuantity = baseQuantity;
            return this;
        }

        /// <summary>
        /// Sets the unit code using the UnitCode enum (recommended).
        /// </summary>
        public ProductBuilder UnitCode(UnitCode unitCode)
        {
            _unitCode = unitCode;
            return this;
        }

        /// <summary>
        /// Sets the unit code using a string code.
        /// The string will be parsed to a UnitCode enum.
        /// </summary>
        public ProductBuilder UnitCode(string unitCode)
        {
            _unitCode = unitCode != null ? UnitCodeExtensions.FromCode(unitCode) : null;
            return this;
        }

        public ProductBuilder UnitPrice(decimal unitPrice)
        {
            _unitPrice = unitPrice;
            return this;
        }

        public ProductBuilder UnitPrice(string unitPrice)
        {
            _unitPrice = decimal.Parse(unitPrice);
            return this;
        }

        public ProductBuilder Discounts(List<Discount> discounts)
        {
            _discounts = discounts ?? new List<Discount>();
            return this;
        }

        public ProductBuilder AddDiscount(Discount discount)
        {
            _discounts.Add(discount);
            return this;
        }

        public ProductBuilder AddDiscount(decimal amount, string label)
        {
            _discounts.Add(Discount.Builder()
                .Amount(amount)
                .Label(label)
                .Build());
            return this;
        }

        public ProductBuilder AddDiscount(string amount, string label)
        {
            return AddDiscount(decimal.Parse(amount), label);
        }

        public ProductBuilder AddDiscount(decimal amount, double percentage, string label)
        {
            _discounts.Add(Discount.Builder()
                .Amount(amount)
                .Percentage(percentage)
                .Label(label)
                .Build());
            return this;
        }

        public ProductBuilder AddDiscount(string amount, double percentage, string label)
        {
            return AddDiscount(decimal.Parse(amount), percentage, label);
        }

        public ProductBuilder Charges(List<Charge> charges)
        {
            _charges = charges ?? new List<Charge>();
            return this;
        }

        public ProductBuilder AddCharge(Charge charge)
        {
            _charges.Add(charge);
            return this;
        }

        public ProductBuilder AddCharge(decimal amount, string label)
        {
            _charges.Add(Charge.Builder()
                .Amount(amount)
                .Label(label)
                .Build());
            return this;
        }

        public ProductBuilder AddCharge(string amount, string label)
        {
            return AddCharge(decimal.Parse(amount), label);
        }

        public ProductBuilder AddCharge(decimal amount, double percentage, string label)
        {
            _charges.Add(Charge.Builder()
                .Amount(amount)
                .Percentage(percentage)
                .Label(label)
                .Build());
            return this;
        }

        public ProductBuilder AddCharge(string amount, double percentage, string label)
        {
            return AddCharge(decimal.Parse(amount), percentage, label);
        }

        public ProductBuilder Taxes(List<Tax> taxes)
        {
            _taxes = taxes ?? new List<Tax>();
            return this;
        }

        public ProductBuilder AddTax(Tax tax)
        {
            _taxes.Add(tax);
            return this;
        }

        public ProductBuilder AddTax(double rate, string type)
        {
            _taxes.Add(Tax.Builder()
                .Rate(rate)
                .Type(type)
                .Build());
            return this;
        }

        public ProductBuilder AddTax(double rate, string type, decimal amount)
        {
            _taxes.Add(Tax.Builder()
                .Rate(rate)
                .Type(type)
                .Amount(amount)
                .Build());
            return this;
        }

        public ProductBuilder AddTax(double rate, string type, string amount)
        {
            _taxes.Add(Tax.Builder()
                .Rate(rate)
                .Type(type)
                .Amount(amount)
                .Build());
            return this;
        }

        public ProductBuilder Subtotal(decimal subtotal)
        {
            _subtotal = subtotal;
            return this;
        }

        public ProductBuilder Subtotal(string subtotal)
        {
            _subtotal = decimal.Parse(subtotal);
            return this;
        }

        public ProductBuilder Total(decimal total)
        {
            _total = total;
            return this;
        }

        public ProductBuilder Total(string total)
        {
            _total = decimal.Parse(total);
            return this;
        }

        /// <summary>
        /// Sets the period for time-based products (subscriptions, rentals, etc.).
        /// </summary>
        public ProductBuilder Period(Period? period)
        {
            _period = period;
            return this;
        }

        public Product Build()
        {
            return new Product(
                _name ?? string.Empty,
                _identifier,
                _description,
                _brand,
                _quantity,
                _baseQuantity,
                _unitCode,
                _unitPrice,
                _discounts,
                _charges,
                _taxes,
                _subtotal,
                _total,
                _period
            );
        }
    }

    // ===== SIMPLE VALIDATION =====

    /// <summary>
    /// Returns true if required fields are present and non-negative.
    /// </summary>
    public bool IsValid() => GetValidationErrors().Count == 0;

    /// <summary>
    /// Very lightweight validation meant as a helper for integrators.
    /// </summary>
    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("name is required");

        if (quantity == null)
            errors.Add("quantity is required");

        if (baseQuantity == null)
            errors.Add("baseQuantity is required (use 1.0 for simple items, or the package size for pre-packaged items)");

        if (unitCode == null)
            errors.Add("unitCode is required (e.g., UnitCode.ONE, UnitCode.KILOGRAM)");

        if (unitPrice == null)
            errors.Add("unitPrice is required");

        if (subtotal == null)
            errors.Add("subtotal is required");

        if (total == null)
            errors.Add("total is required");

        if (quantity != null && quantity <= 0)
            errors.Add("quantity must be greater than 0");

        if (baseQuantity != null && baseQuantity <= 0)
            errors.Add("baseQuantity must be greater than 0");

        return errors;
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not Product other) return false;
        return name == other.name
            && identifier == other.identifier
            && description == other.description
            && brand == other.brand
            && quantity == other.quantity
            && baseQuantity == other.baseQuantity
            && unitCode == other.unitCode
            && unitPrice == other.unitPrice
            && discounts.SequenceEqual(other.discounts)
            && charges.SequenceEqual(other.charges)
            && taxes.SequenceEqual(other.taxes)
            && subtotal == other.subtotal
            && total == other.total
            && Equals(period, other.period);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(name);
        hash.Add(identifier);
        hash.Add(description);
        hash.Add(brand);
        hash.Add(quantity);
        hash.Add(baseQuantity);
        hash.Add(unitCode);
        hash.Add(unitPrice);
        hash.Add(discounts);
        hash.Add(charges);
        hash.Add(taxes);
        hash.Add(subtotal);
        hash.Add(total);
        hash.Add(period);
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        return $"Product{{name='{name}', identifier='{identifier}', quantity={quantity}, " +
               $"unitPrice={unitPrice}, discounts={discounts.Count}, charges={charges.Count}, " +
               $"taxes={taxes.Count}, subtotal={subtotal}, total={total}}}";
    }
}
