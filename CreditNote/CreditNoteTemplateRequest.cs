using System.Globalization;
using System.Text.Json.Serialization;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Enums;
using Identifier = Cheqi.Sdk.Models.Generated.Identifier;

namespace Cheqi.Sdk.CreditNote;

/// <summary>
/// Request for generating a credit note template.
/// 
/// <para>This class provides a simplified API for creating credit notes.
/// The POS system provides all calculated values - no calculations are performed by the SDK.</para>
/// 
/// <example>
/// Example Usage:
/// <code>
/// var creditNote = CreditNoteTemplateRequest.Builder()
///     // Basic information
///     .DocumentNumber("CN-2024-001")
///     .OriginatorDocumentReference("INV-2024-001")  // Original receipt
///     .IssueDate(DateTimeOffset.Now)
///     .Currency("EUR")
///     
///     // Totals (POS-calculated, negative for refunds)
///     .CreditNoteSubTotal(-100.00m)
///     .TotalBeforeTax(-100.00m)
///     .TotalTaxAmount(-21.00m)
///     .TotalAmount(-121.00m)
///     
///     // Simple product addition (POS provides all amounts)
///     .AddProduct("Nike", "AirMax", 125.00m, -125.00m, 21.0, -26.25m, -151.25m, -1.0)
///     
///     // Or use full Product builder
///     .AddProduct(Product.Builder()
///         .Name("Laptop")
///         .Quantity(-1.0)  // Negative for returns
///         .UnitCode(UnitCode.ONE)
///         .UnitPrice("100.00")
///         .Subtotal("-100.00")
///         .Total("-121.00")
///         .AddTax(21.0, "VAT", "-21.00")
///         .Build())
///     
///     // Tax breakdown
///     .AddTax(21.0, "VAT", "-21.00")
///     
///     .Note("Refund for returned item")
///     .Build();
/// </code>
/// </example>
/// 
/// <para><b>Mandatory Fields:</b></para>
/// <list type="bullet">
///   <item><b>documentNumber</b>: Unique credit note identifier</item>
///   <item><b>originatorDocumentReference</b>: Reference to original receipt</item>
///   <item><b>issueDate</b>: Credit note issue date</item>
///   <item><b>currency</b>: ISO 4217 currency code</item>
///   <item><b>creditNoteSubTotal</b>: Sum of all product line totals</item>
///   <item><b>totalBeforeTax</b>: Total excluding taxes</item>
///   <item><b>totalTaxAmount</b>: Total tax amount</item>
///   <item><b>totalAmount</b>: Final amount</item>
///   <item><b>products</b>: List of products (minimum 1 required)</item>
///   <item><b>taxes</b>: Tax breakdown by category/rate</item>
/// </list>
/// </summary>
public sealed class CreditNoteTemplateRequest
{
    // ===== PROPERTIES =====

    /// <summary>
    /// Unique credit note/document number from the POS system.
    /// </summary>
    [JsonPropertyName("documentNumber")]
    public string documentNumber { get; }

    [JsonIgnore]
    public string CreditNoteId => documentNumber;

    [JsonIgnore]
    public string DocumentNumber => documentNumber;

    /// <summary>
    /// Reference to the original receipt/document.
    /// </summary>
    [JsonPropertyName("originatorDocumentReference")]
    public string originatorDocumentReference { get; }

    [JsonIgnore]
    public string OriginatorDocumentReference => originatorDocumentReference;

    /// <summary>
    /// Additional identifiers for the credit note.
    /// </summary>
    [JsonPropertyName("identifiers")]
    public IReadOnlyList<Identifier> identifiers { get; }

    /// <summary>
    /// Issue date/time of the credit note.
    /// </summary>
    [JsonPropertyName("issueDate")]
    public DateTimeOffset issueDate { get; }

    /// <summary>
    /// Currency code (ISO 4217, e.g., "EUR").
    /// </summary>
    [JsonPropertyName("currency")]
    public string currency { get; }

    /// <summary>
    /// Credit note subtotal.
    /// </summary>
    [JsonPropertyName("creditNoteSubTotal")]
    public decimal creditNoteSubTotal { get; }

    /// <summary>
    /// Total credit amount excluding tax.
    /// </summary>
    [JsonPropertyName("totalBeforeTax")]
    public decimal totalBeforeTax { get; }

    /// <summary>
    /// Total tax amount.
    /// </summary>
    [JsonPropertyName("totalTaxAmount")]
    public decimal totalTaxAmount { get; }

    /// <summary>
    /// Total credit amount including tax.
    /// </summary>
    [JsonPropertyName("totalAmount")]
    public decimal totalAmount { get; }

    /// <summary>
    /// Line items being credited.
    /// </summary>
    [JsonPropertyName("products")]
    public IReadOnlyList<Product> products { get; }

    /// <summary>
    /// Credit note-level discounts.
    /// </summary>
    [JsonPropertyName("discounts")]
    public IReadOnlyList<Discount> discounts { get; }

    /// <summary>
    /// Tax breakdown by category.
    /// </summary>
    [JsonPropertyName("taxes")]
    public IReadOnlyList<Tax> taxes { get; }

    /// <summary>
    /// Additional note or reason for the credit note.
    /// </summary>
    [JsonPropertyName("note")]
    public string? note { get; }

    // ===== CONSTRUCTOR =====

    private CreditNoteTemplateRequest(
        string documentNumber,
        string originatorDocumentReference,
        List<Identifier>? identifiers,
        DateTimeOffset issueDate,
        string currency,
        decimal creditNoteSubTotal,
        decimal totalBeforeTax,
        decimal totalTaxAmount,
        decimal totalAmount,
        List<Product>? products,
        List<Discount>? discounts,
        List<Tax>? taxes,
        string? note)
    {
        this.documentNumber = documentNumber;
        this.originatorDocumentReference = originatorDocumentReference;
        this.identifiers = identifiers?.AsReadOnly() ?? new List<Identifier>().AsReadOnly();
        this.issueDate = issueDate;
        this.currency = currency;
        this.creditNoteSubTotal = creditNoteSubTotal;
        this.totalBeforeTax = totalBeforeTax;
        this.totalTaxAmount = totalTaxAmount;
        this.totalAmount = totalAmount;
        this.products = products?.AsReadOnly() ?? new List<Product>().AsReadOnly();
        this.discounts = discounts?.AsReadOnly() ?? new List<Discount>().AsReadOnly();
        this.taxes = taxes?.AsReadOnly() ?? new List<Tax>().AsReadOnly();
        this.note = note;
    }

    // ===== BUILDER =====

    public static CreditNoteTemplateRequestBuilder Builder() => new CreditNoteTemplateRequestBuilder();

    public sealed class CreditNoteTemplateRequestBuilder
    {
        private string? _documentNumber;
        private string? _originatorDocumentReference;
        private List<Identifier>? _identifiers;
        private DateTimeOffset? _issueDate;
        private string? _currency;
        private decimal? _creditNoteSubTotal;
        private decimal? _totalBeforeTax;
        private decimal? _totalTaxAmount;
        private decimal? _totalAmount;
        private List<Product>? _products;
        private List<Discount>? _discounts;
        private List<Tax>? _taxes;
        private string? _note;

        internal CreditNoteTemplateRequestBuilder() { }

        public CreditNoteTemplateRequestBuilder From(CreditNoteTemplateRequest other)
        {
            _documentNumber = other.documentNumber;
            _originatorDocumentReference = other.originatorDocumentReference;
            _identifiers = other.identifiers.ToList();
            _issueDate = other.issueDate;
            _currency = other.currency;
            _creditNoteSubTotal = other.creditNoteSubTotal;
            _totalBeforeTax = other.totalBeforeTax;
            _totalTaxAmount = other.totalTaxAmount;
            _totalAmount = other.totalAmount;
            _products = other.products.ToList();
            _discounts = other.discounts.ToList();
            _taxes = other.taxes.ToList();
            _note = other.note;
            return this;
        }

        public CreditNoteTemplateRequestBuilder DocumentNumber(string documentNumber)
        {
            _documentNumber = documentNumber;
            return this;
        }

        public CreditNoteTemplateRequestBuilder OriginatorDocumentReference(string originatorDocumentReference)
        {
            _originatorDocumentReference = originatorDocumentReference;
            return this;
        }

        public CreditNoteTemplateRequestBuilder Identifiers(List<Identifier> identifiers)
        {
            _identifiers = identifiers;
            return this;
        }

        public CreditNoteTemplateRequestBuilder IssueDate(DateTimeOffset issueDate)
        {
            _issueDate = issueDate;
            return this;
        }

        /// <summary>
        /// Sets the currency code (ISO 4217).
        /// </summary>
        /// <param name="currency">ISO 4217 currency code (e.g., "EUR", "USD", "GBP")</param>
        /// <returns>This builder</returns>
        /// <exception cref="ArgumentException">if the currency code is invalid</exception>
        public CreditNoteTemplateRequestBuilder Currency(string currency)
        {
            if (currency != null)
            {
                try
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
                catch (ArgumentException)
                {
                    throw;
                }
            }
            _currency = currency;
            return this;
        }

        public CreditNoteTemplateRequestBuilder CreditNoteSubTotal(decimal creditNoteSubTotal)
        {
            _creditNoteSubTotal = creditNoteSubTotal;
            return this;
        }

        public CreditNoteTemplateRequestBuilder TotalBeforeTax(decimal totalBeforeTax)
        {
            _totalBeforeTax = totalBeforeTax;
            return this;
        }

        public CreditNoteTemplateRequestBuilder TotalBeforeTax(string totalBeforeTax)
        {
            _totalBeforeTax = decimal.Parse(totalBeforeTax);
            return this;
        }

        public CreditNoteTemplateRequestBuilder TotalTaxAmount(decimal totalTaxAmount)
        {
            _totalTaxAmount = totalTaxAmount;
            return this;
        }

        public CreditNoteTemplateRequestBuilder TotalTaxAmount(string totalTaxAmount)
        {
            _totalTaxAmount = decimal.Parse(totalTaxAmount);
            return this;
        }

        public CreditNoteTemplateRequestBuilder TotalAmount(decimal totalAmount)
        {
            _totalAmount = totalAmount;
            return this;
        }

        public CreditNoteTemplateRequestBuilder TotalAmount(string totalAmount)
        {
            _totalAmount = decimal.Parse(totalAmount);
            return this;
        }

        public CreditNoteTemplateRequestBuilder Products(List<Product> products)
        {
            _products = products;
            return this;
        }

        /// <summary>
        /// Convenience method to add a single product to the credit note.
        /// </summary>
        public CreditNoteTemplateRequestBuilder AddProduct(Product product)
        {
            _products ??= new List<Product>();
            _products.Add(product);
            return this;
        }

        /// <summary>
        /// Convenience method to add a simple product with basic details.
        /// All amounts must be pre-calculated by the POS system.
        /// </summary>
        /// <param name="brand">Product brand/manufacturer</param>
        /// <param name="name">Product name/model</param>
        /// <param name="unitPrice">Price per unit</param>
        /// <param name="subtotal">Subtotal (unitPrice × quantity, pre-calculated)</param>
        /// <param name="taxRate">Tax rate as percentage (e.g., 21.0 for 21%)</param>
        /// <param name="taxAmount">Tax amount (pre-calculated)</param>
        /// <param name="total">Total amount including tax (pre-calculated)</param>
        /// <param name="quantity">Number of units (negative for returns)</param>
        /// <returns>This builder</returns>
        public CreditNoteTemplateRequestBuilder AddProduct(string brand, string name, string unitPrice, string subtotal, double taxRate, string taxAmount, string total, double quantity)
        {
            return AddProduct(Product.Builder()
                .BrandName(brand)
                .Name(name)
                .Quantity(quantity)
                .BaseQuantity(1.0)
                .UnitCode(UnitCode.ONE)
                .UnitPrice(unitPrice)
                .Subtotal(subtotal)
                .Total(total)
                .AddTax(taxRate, "VAT", taxAmount)
                .Build());
        }

        /// <summary>
        /// Convenience method with decimal amounts.
        /// </summary>
        public CreditNoteTemplateRequestBuilder AddProduct(string brand, string name, decimal unitPrice, decimal subtotal, double taxRate, decimal taxAmount, decimal total, double quantity)
        {
            return AddProduct(brand, name, unitPrice.ToString(), subtotal.ToString(), taxRate, taxAmount.ToString(), total.ToString(), quantity);
        }

        public CreditNoteTemplateRequestBuilder Discounts(List<Discount> discounts)
        {
            _discounts = discounts;
            return this;
        }

        /// <summary>
        /// Convenience method to add a single discount to the credit note.
        /// </summary>
        public CreditNoteTemplateRequestBuilder AddDiscount(Discount discount)
        {
            _discounts ??= new List<Discount>();
            _discounts.Add(discount);
            return this;
        }

        /// <summary>
        /// Convenience method to add a discount with amount and label.
        /// </summary>
        public CreditNoteTemplateRequestBuilder AddDiscount(decimal amount, string label)
        {
            return AddDiscount(Discount.Of(amount, label));
        }

        /// <summary>
        /// Convenience method to add a discount with amount (string) and label.
        /// </summary>
        public CreditNoteTemplateRequestBuilder AddDiscount(string amount, string label)
        {
            return AddDiscount(Discount.Of(decimal.Parse(amount), label));
        }

        public CreditNoteTemplateRequestBuilder Taxes(List<Tax> taxes)
        {
            _taxes = taxes;
            return this;
        }

        /// <summary>
        /// Convenience method to add a single tax entry.
        /// </summary>
        public CreditNoteTemplateRequestBuilder AddTax(Tax tax)
        {
            _taxes ??= new List<Tax>();
            _taxes.Add(tax);
            return this;
        }

        /// <summary>
        /// Convenience method to add a tax with rate, type, and amount.
        /// </summary>
        public CreditNoteTemplateRequestBuilder AddTax(double rate, string type, decimal amount)
        {
            return AddTax(Tax.Builder()
                .Rate(rate)
                .Type(type)
                .Amount(amount)
                .Build());
        }

        /// <summary>
        /// Convenience method to add a tax with rate, type, and amount (string).
        /// </summary>
        public CreditNoteTemplateRequestBuilder AddTax(double rate, string type, string amount)
        {
            return AddTax(rate, type, decimal.Parse(amount));
        }

        public CreditNoteTemplateRequestBuilder Note(string? note)
        {
            _note = note;
            return this;
        }

        public CreditNoteTemplateRequest Build()
        {
            if (_documentNumber == null)
                throw new InvalidOperationException("DocumentNumber is required");
            if (_originatorDocumentReference == null)
                throw new InvalidOperationException("OriginatorDocumentReference is required");
            if (_issueDate == null)
                throw new InvalidOperationException("IssueDate is required");
            if (_currency == null)
                throw new InvalidOperationException("Currency is required");
            if (_creditNoteSubTotal == null)
                throw new InvalidOperationException("CreditNoteSubTotal is required");
            if (_totalBeforeTax == null)
                throw new InvalidOperationException("TotalBeforeTax is required");
            if (_totalTaxAmount == null)
                throw new InvalidOperationException("TotalTaxAmount is required");
            if (_totalAmount == null)
                throw new InvalidOperationException("TotalAmount is required");

            return new CreditNoteTemplateRequest(
                _documentNumber,
                _originatorDocumentReference,
                _identifiers,
                _issueDate.Value,
                _currency,
                _creditNoteSubTotal.Value,
                _totalBeforeTax.Value,
                _totalTaxAmount.Value,
                _totalAmount.Value,
                _products,
                _discounts,
                _taxes,
                _note
            );
        }
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not CreditNoteTemplateRequest other) return false;
        return documentNumber == other.documentNumber
            && originatorDocumentReference == other.originatorDocumentReference
            && identifiers.SequenceEqual(other.identifiers)
            && issueDate == other.issueDate
            && currency == other.currency
            && creditNoteSubTotal == other.creditNoteSubTotal
            && totalBeforeTax == other.totalBeforeTax
            && totalTaxAmount == other.totalTaxAmount
            && totalAmount == other.totalAmount
            && products.SequenceEqual(other.products)
            && discounts.SequenceEqual(other.discounts)
            && taxes.SequenceEqual(other.taxes)
            && note == other.note;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(documentNumber);
        hash.Add(originatorDocumentReference);
        hash.Add(issueDate);
        hash.Add(currency);
        hash.Add(creditNoteSubTotal);
        hash.Add(totalBeforeTax);
        hash.Add(totalTaxAmount);
        hash.Add(totalAmount);
        hash.Add(note);
        foreach (var identifier in identifiers) hash.Add(identifier);
        foreach (var product in products) hash.Add(product);
        foreach (var discount in discounts) hash.Add(discount);
        foreach (var tax in taxes) hash.Add(tax);
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        return $"CreditNoteTemplateRequest{{" +
               $"documentNumber='{documentNumber}', " +
               $"originatorDocumentReference='{originatorDocumentReference}', " +
               $"identifiers={identifiers.Count} items, " +
               $"issueDate={issueDate}, " +
               $"currency='{currency}', " +
               $"creditNoteSubTotal={creditNoteSubTotal}, " +
               $"totalBeforeTax={totalBeforeTax}, " +
               $"totalTaxAmount={totalTaxAmount}, " +
               $"totalAmount={totalAmount}, " +
               $"products={products.Count} items, " +
               $"discounts={discounts.Count} items, " +
               $"taxes={taxes.Count} items, " +
               $"note='{note}'}}";
    }
}
