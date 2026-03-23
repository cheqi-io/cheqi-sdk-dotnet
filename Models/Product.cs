using Cheqi.Sdk.Models.Enums;
using GeneratedModels = Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Product line item for Cheqi receipts.
/// Extends the generated base with a builder pattern and convenience methods.
///
/// <para><b>Example:</b></para>
/// <code>
/// var laptop = Product.Builder()
///     .Name("Laptop 13\"")
///     .Identifier("LAP-001")
///     .Quantity(1.0)
///     .BaseQuantity(1.0)
///     .UnitCode(UnitCode.ONE)
///     .UnitPrice(1000.00m)
///     .Subtotal(1000.00m)
///     .Total(1210.00m)
///     .AddTax(21.0, "VAT", "1000.00", "210.00")
///     .Build();
/// </code>
/// </summary>
public sealed class Product : GeneratedModels.Product
{
    public Product() : base("", "", "", 0, default, 0, new List<GeneratedModels.Tax>(), 0, 0) { }

    // ===== BUILDER =====

    public static ProductBuilder Builder() => new ProductBuilder();

    public sealed class ProductBuilder
    {
        private string? _name;
        private string? _identifier;
        private string? _description;
        private string? _brandName;
        private double? _quantity;
        private double? _baseQuantity;
        private GeneratedModels.UnitCode? _unitCode;
        private decimal? _unitPrice;
        private List<GeneratedModels.Discount> _discounts = new();
        private List<GeneratedModels.Charge> _charges = new();
        private List<GeneratedModels.Tax> _taxes = new();
        private List<GeneratedModels.Barcode> _barcodes = new();
        private decimal? _subtotal;
        private decimal? _total;
        private GeneratedModels.Period? _period;

        internal ProductBuilder() { }

        public ProductBuilder From(GeneratedModels.Product other)
        {
            _name = other.Name;
            _identifier = other.Identifier;
            _description = other.Description;
            _brandName = other.BrandName;
            _quantity = other.Quantity;
            _baseQuantity = other.BaseQuantity;
            _unitCode = other.UnitCode;
            _unitPrice = other.UnitPrice;
            _discounts = other.Discounts != null ? new List<GeneratedModels.Discount>(other.Discounts) : new();
            _charges = other.Charges != null ? new List<GeneratedModels.Charge>(other.Charges) : new();
            _taxes = other.Taxes != null ? new List<GeneratedModels.Tax>(other.Taxes) : new();
            _barcodes = other.Barcodes != null ? new List<GeneratedModels.Barcode>(other.Barcodes) : new();
            _subtotal = other.Subtotal;
            _total = other.Total;
            _period = other.Period;
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

        public ProductBuilder BrandName(string? brandName)
        {
            _brandName = brandName;
            return this;
        }

        public ProductBuilder Quantity(double quantity)
        {
            _quantity = quantity;
            return this;
        }

        public ProductBuilder BaseQuantity(double baseQuantity)
        {
            _baseQuantity = baseQuantity;
            return this;
        }

        public ProductBuilder UnitCode(GeneratedModels.UnitCode unitCode)
        {
            _unitCode = unitCode;
            return this;
        }

        public ProductBuilder UnitCode(Enums.UnitCode unitCode)
        {
            var code = unitCode.GetCode();
            _unitCode = GeneratedModels.UnitCodeValueConverter.FromStringOrDefault(code);
            return this;
        }

        public ProductBuilder UnitCode(string unitCode)
        {
            _unitCode = GeneratedModels.UnitCodeValueConverter.FromStringOrDefault(unitCode);
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

        // ===== DISCOUNTS =====

        public ProductBuilder Discounts(List<GeneratedModels.Discount> discounts)
        {
            _discounts = discounts ?? new();
            return this;
        }

        public ProductBuilder AddDiscount(GeneratedModels.Discount discount)
        {
            _discounts.Add(discount);
            return this;
        }

        public ProductBuilder AddDiscount(decimal amount, string label)
        {
            _discounts.Add(Discount.Of(amount, label));
            return this;
        }

        public ProductBuilder AddDiscount(string amount, string label)
        {
            return AddDiscount(decimal.Parse(amount), label);
        }

        public ProductBuilder AddDiscount(decimal amount, double percentage, string label)
        {
            _discounts.Add(Discount.Of(amount, percentage, label));
            return this;
        }

        public ProductBuilder AddDiscount(string amount, double percentage, string label)
        {
            return AddDiscount(decimal.Parse(amount), percentage, label);
        }

        // ===== CHARGES =====

        public ProductBuilder Charges(List<GeneratedModels.Charge> charges)
        {
            _charges = charges ?? new();
            return this;
        }

        public ProductBuilder AddCharge(GeneratedModels.Charge charge)
        {
            _charges.Add(charge);
            return this;
        }

        public ProductBuilder AddCharge(decimal amount, string label)
        {
            _charges.Add(Charge.Of(amount, label));
            return this;
        }

        public ProductBuilder AddCharge(string amount, string label)
        {
            return AddCharge(decimal.Parse(amount), label);
        }

        public ProductBuilder AddCharge(decimal amount, double percentage, string label)
        {
            _charges.Add(Charge.Of(amount, percentage, label));
            return this;
        }

        public ProductBuilder AddCharge(string amount, double percentage, string label)
        {
            return AddCharge(decimal.Parse(amount), percentage, label);
        }

        // ===== TAXES =====

        public ProductBuilder Taxes(List<GeneratedModels.Tax> taxes)
        {
            _taxes = taxes ?? new();
            return this;
        }

        public ProductBuilder AddTax(GeneratedModels.Tax tax)
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

        public ProductBuilder AddTax(double rate, string type, decimal taxableAmount, decimal amount)
        {
            _taxes.Add(Tax.Builder()
                .Rate(rate)
                .Type(type)
                .TaxableAmount(taxableAmount)
                .Amount(amount)
                .Build());
            return this;
        }

        public ProductBuilder AddTax(double rate, string type, string taxableAmount, string amount)
        {
            _taxes.Add(Tax.Builder()
                .Rate(rate)
                .Type(type)
                .TaxableAmount(taxableAmount)
                .Amount(amount)
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

        // ===== BARCODES =====

        public ProductBuilder Barcodes(List<GeneratedModels.Barcode> barcodes)
        {
            _barcodes = barcodes ?? new();
            return this;
        }

        public ProductBuilder AddBarcode(GeneratedModels.Barcode barcode)
        {
            _barcodes.Add(barcode);
            return this;
        }

        // ===== TOTALS & PERIOD =====

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

        public ProductBuilder Period(GeneratedModels.Period? period)
        {
            _period = period;
            return this;
        }

        public Product Build()
        {
            var p = new Product();
            p.Name = _name ?? string.Empty;
            p.Identifier = _identifier ?? string.Empty;
            p.Description = _description;
            p.BrandName = _brandName ?? string.Empty;
            p.Quantity = _quantity ?? 0;
            p.BaseQuantity = _baseQuantity;
            p.UnitCode = _unitCode ?? default;
            p.UnitPrice = _unitPrice ?? 0;
            p.Discounts = _discounts.Count > 0 ? _discounts : null;
            p.Charges = _charges.Count > 0 ? _charges : null;
            p.Taxes = _taxes.Count > 0 ? _taxes : new List<GeneratedModels.Tax>();
            p.Barcodes = _barcodes.Count > 0 ? _barcodes : null;
            p.Subtotal = _subtotal ?? 0;
            p.Total = _total ?? 0;
            p.Period = _period;
            return p;
        }
    }

    // ===== VALIDATION =====

    public bool IsValid() => GetValidationErrors().Count == 0;

    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("name is required");
        if (Quantity <= 0)
            errors.Add("quantity must be greater than 0");

        return errors;
    }
}
