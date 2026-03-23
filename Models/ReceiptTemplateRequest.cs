using System.Globalization;
using Cheqi.Sdk.Models.Enums;
using GeneratedModels = Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Request DTO for generating receipt templates.
/// Extends the generated base with a builder pattern and convenience methods.
///
/// <para><b>Example:</b></para>
/// <code>
/// var receipt = ReceiptTemplateRequest.Builder()
///     .DocumentNumber("INV-2024-001")
///     .IssueDate(DateTimeOffset.Now)
///     .Currency("EUR")
///     .ReceiptSubtotal(250.00m)
///     .TotalBeforeTax(245.00m)
///     .TotalTaxAmount(51.45m)
///     .TotalAmount(296.45m)
///     .AddProduct(Product.Builder()
///         .Name("Laptop")
///         .Quantity(1.0)
///         .UnitCode(UnitCode.ONE)
///         .UnitPrice(100.00m)
///         .Subtotal(100.00m)
///         .Total(121.00m)
///         .AddTax(21.0, "VAT", "21.00")
///         .Build())
///     .AddTax(Tax.Builder()
///         .Rate(21.0)
///         .Type("VAT")
///         .Amount(51.45m)
///         .Build())
///     .Build();
/// </code>
/// </summary>
public class ReceiptTemplateRequest : GeneratedModels.ReceiptTemplateRequest
{
    public ReceiptTemplateRequest()
        : base("", default, "", 0, 0, 0, 0, new List<GeneratedModels.Product>()) { }

    // ===== BUILDER =====

    public static ReceiptTemplateRequestBuilder Builder() => new ReceiptTemplateRequestBuilder();

    public sealed class ReceiptTemplateRequestBuilder
    {
        private Guid? _childCompanyId;
        private string? _documentNumber;
        private List<GeneratedModels.Identifier>? _identifiers;
        private DateTimeOffset? _issueDate;
        private string? _currency;
        private decimal? _receiptSubtotal;
        private decimal? _totalBeforeTax;
        private decimal? _totalTaxAmount;
        private decimal? _totalAmount;
        private List<GeneratedModels.Product>? _products;
        private List<GeneratedModels.Discount>? _discounts;
        private List<GeneratedModels.Charge>? _charges;
        private List<GeneratedModels.Tax>? _taxes;
        private DateTimeOffset? _transactionDate;
        private DateTimeOffset? _purchaseDate;
        private GeneratedModels.Period? _period;
        private string? _note;

        internal ReceiptTemplateRequestBuilder() { }

        public ReceiptTemplateRequestBuilder From(GeneratedModels.ReceiptTemplateRequest other)
        {
            _childCompanyId = other.ChildCompanyId;
            _documentNumber = other.DocumentNumber;
            _identifiers = other.Identifiers?.ToList();
            _issueDate = other.IssueDate;
            _currency = other.Currency;
            _receiptSubtotal = other.ReceiptSubtotal;
            _totalBeforeTax = other.TotalBeforeTax;
            _totalTaxAmount = other.TotalTaxAmount;
            _totalAmount = other.TotalAmount;
            _products = other.Products?.ToList();
            _discounts = other.Discounts?.ToList();
            _charges = other.Charges?.ToList();
            _taxes = other.Taxes?.ToList();
            _transactionDate = other.TransactionDate;
            _purchaseDate = other.PurchaseDate;
            _period = other.Period;
            _note = other.Note;
            return this;
        }

        public ReceiptTemplateRequestBuilder ChildCompanyId(Guid? childCompanyId)
        {
            _childCompanyId = childCompanyId;
            return this;
        }

        public ReceiptTemplateRequestBuilder DocumentNumber(string documentNumber)
        {
            _documentNumber = documentNumber;
            return this;
        }

        public ReceiptTemplateRequestBuilder Identifiers(List<GeneratedModels.Identifier> identifiers)
        {
            _identifiers = identifiers;
            return this;
        }

        public ReceiptTemplateRequestBuilder IssueDate(DateTimeOffset issueDate)
        {
            _issueDate = issueDate;
            return this;
        }

        public ReceiptTemplateRequestBuilder Currency(string currency)
        {
            if (currency != null)
            {
                var region = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .Select(c => new RegionInfo(c.Name))
                    .FirstOrDefault(r => r.ISOCurrencySymbol == currency);

                if (region == null)
                {
                    throw new ArgumentException(
                        $"Invalid currency code: '{currency}'. Must be a valid ISO 4217 code (e.g., EUR, USD, GBP)");
                }
            }
            _currency = currency;
            return this;
        }

        public ReceiptTemplateRequestBuilder ReceiptSubtotal(decimal receiptSubtotal)
        {
            _receiptSubtotal = receiptSubtotal;
            return this;
        }

        public ReceiptTemplateRequestBuilder TotalBeforeTax(decimal totalBeforeTax)
        {
            _totalBeforeTax = totalBeforeTax;
            return this;
        }

        public ReceiptTemplateRequestBuilder TotalBeforeTax(string totalBeforeTax)
        {
            _totalBeforeTax = decimal.Parse(totalBeforeTax);
            return this;
        }

        public ReceiptTemplateRequestBuilder TotalTaxAmount(decimal totalTaxAmount)
        {
            _totalTaxAmount = totalTaxAmount;
            return this;
        }

        public ReceiptTemplateRequestBuilder TotalTaxAmount(string totalTaxAmount)
        {
            _totalTaxAmount = decimal.Parse(totalTaxAmount);
            return this;
        }

        public ReceiptTemplateRequestBuilder TotalAmount(decimal totalAmount)
        {
            _totalAmount = totalAmount;
            return this;
        }

        public ReceiptTemplateRequestBuilder TotalAmount(string totalAmount)
        {
            _totalAmount = decimal.Parse(totalAmount);
            return this;
        }

        public ReceiptTemplateRequestBuilder Products(List<GeneratedModels.Product> products)
        {
            _products = products;
            return this;
        }

        public ReceiptTemplateRequestBuilder AddProduct(GeneratedModels.Product product)
        {
            _products ??= new List<GeneratedModels.Product>();
            _products.Add(product);
            return this;
        }

        public ReceiptTemplateRequestBuilder AddProduct(string identifier, string brand, string name,
            string unitPrice, string subtotal, double taxRate, string taxAmount, string total, double quantity)
        {
            return AddProduct(Product.Builder()
                .BrandName(brand)
                .Name(name)
                .Identifier(identifier)
                .Quantity(quantity)
                .BaseQuantity(1.0)
                .UnitCode(GeneratedModels.UnitCode.C62)
                .UnitPrice(unitPrice)
                .Subtotal(subtotal)
                .Total(total)
                .AddTax(taxRate, "VAT", taxAmount)
                .Build());
        }

        public ReceiptTemplateRequestBuilder AddProduct(string identifier, string brand, string name,
            decimal unitPrice, decimal subtotal, double taxRate, decimal taxAmount, decimal total, double quantity)
        {
            return AddProduct(identifier, brand, name, unitPrice.ToString(), subtotal.ToString(),
                taxRate, taxAmount.ToString(), total.ToString(), quantity);
        }

        public ReceiptTemplateRequestBuilder Discounts(List<GeneratedModels.Discount> discounts)
        {
            _discounts = discounts;
            return this;
        }

        public ReceiptTemplateRequestBuilder AddDiscount(GeneratedModels.Discount discount)
        {
            _discounts ??= new List<GeneratedModels.Discount>();
            _discounts.Add(discount);
            return this;
        }

        public ReceiptTemplateRequestBuilder AddDiscount(decimal amount, string label)
        {
            return AddDiscount(Discount.Of(amount, label));
        }

        public ReceiptTemplateRequestBuilder AddDiscount(string amount, string label)
        {
            return AddDiscount(Discount.Of(decimal.Parse(amount), label));
        }

        public ReceiptTemplateRequestBuilder AddDiscount(decimal amount, double percentage, string label)
        {
            return AddDiscount(Discount.Of(amount, percentage, label));
        }

        public ReceiptTemplateRequestBuilder AddDiscount(string amount, double percentage, string label)
        {
            return AddDiscount(Discount.Of(decimal.Parse(amount), percentage, label));
        }

        public ReceiptTemplateRequestBuilder Charges(List<GeneratedModels.Charge> charges)
        {
            _charges = charges;
            return this;
        }

        public ReceiptTemplateRequestBuilder AddCharge(GeneratedModels.Charge charge)
        {
            _charges ??= new List<GeneratedModels.Charge>();
            _charges.Add(charge);
            return this;
        }

        public ReceiptTemplateRequestBuilder AddCharge(decimal amount, string label)
        {
            return AddCharge(Charge.Of(amount, label));
        }

        public ReceiptTemplateRequestBuilder AddCharge(string amount, string label)
        {
            return AddCharge(Charge.Of(decimal.Parse(amount), label));
        }

        public ReceiptTemplateRequestBuilder AddCharge(decimal amount, double percentage, string label)
        {
            return AddCharge(Charge.Of(amount, percentage, label));
        }

        public ReceiptTemplateRequestBuilder AddCharge(string amount, double percentage, string label)
        {
            return AddCharge(Charge.Of(decimal.Parse(amount), percentage, label));
        }

        public ReceiptTemplateRequestBuilder Taxes(List<GeneratedModels.Tax> taxes)
        {
            _taxes = taxes;
            return this;
        }

        public ReceiptTemplateRequestBuilder AddTax(GeneratedModels.Tax tax)
        {
            _taxes ??= new List<GeneratedModels.Tax>();
            _taxes.Add(tax);
            return this;
        }

        public ReceiptTemplateRequestBuilder AddTax(double rate, string type, decimal amount)
        {
            return AddTax(Tax.Builder()
                .Rate(rate)
                .Type(type)
                .Amount(amount)
                .Build());
        }

        public ReceiptTemplateRequestBuilder AddTax(double rate, string type, string amount)
        {
            return AddTax(rate, type, decimal.Parse(amount));
        }

        public ReceiptTemplateRequestBuilder TransactionDate(DateTimeOffset? transactionDate)
        {
            _transactionDate = transactionDate;
            return this;
        }

        public ReceiptTemplateRequestBuilder PurchaseDate(DateTimeOffset? purchaseDate)
        {
            _purchaseDate = purchaseDate;
            return this;
        }

        public ReceiptTemplateRequestBuilder Period(GeneratedModels.Period? period)
        {
            _period = period;
            return this;
        }

        public ReceiptTemplateRequestBuilder Note(string? note)
        {
            _note = note;
            return this;
        }

        public ReceiptTemplateRequest Build()
        {
            if (_documentNumber == null)
                throw new InvalidOperationException("DocumentNumber is required");
            if (_issueDate == null)
                throw new InvalidOperationException("IssueDate is required");
            if (_currency == null)
                throw new InvalidOperationException("Currency is required");
            if (_receiptSubtotal == null)
                throw new InvalidOperationException("ReceiptSubtotal is required");
            if (_totalBeforeTax == null)
                throw new InvalidOperationException("TotalBeforeTax is required");
            if (_totalTaxAmount == null)
                throw new InvalidOperationException("TotalTaxAmount is required");
            if (_totalAmount == null)
                throw new InvalidOperationException("TotalAmount is required");

            var r = new ReceiptTemplateRequest();
            r.ChildCompanyId = _childCompanyId;
            r.DocumentNumber = _documentNumber;
            r.Identifiers = _identifiers;
            r.IssueDate = _issueDate.Value;
            r.Currency = _currency;
            r.ReceiptSubtotal = _receiptSubtotal.Value;
            r.TotalBeforeTax = _totalBeforeTax.Value;
            r.TotalTaxAmount = _totalTaxAmount.Value;
            r.TotalAmount = _totalAmount.Value;
            r.Products = _products ?? new List<GeneratedModels.Product>();
            r.Discounts = _discounts;
            r.Charges = _charges;
            r.Taxes = _taxes;
            r.TransactionDate = _transactionDate;
            r.PurchaseDate = _purchaseDate;
            r.Period = _period;
            r.Note = _note;
            return r;
        }
    }
}
