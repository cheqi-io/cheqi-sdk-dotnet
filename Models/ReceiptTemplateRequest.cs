using System.Globalization;
using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Request DTO for generating receipt templates.
/// 
/// <para>This class provides a simplified, POS-friendly API for creating digital receipts.
/// The POS system provides all calculated values - no calculations are performed by the SDK.</para>
///
/// <para><b>Example Usage:</b></para>
/// <code>
/// var receipt = ReceiptTemplateRequest.Builder()
///     // Basic information
///     .DocumentNumber("INV-2024-001")
///     .IssueDate(DateTimeOffset.Now)
///     .Currency("EUR")
///     
///     // Totals (POS-calculated)
///     .InvoiceSubtotal(250.00m)
///     .TotalBeforeTax(245.00m)  // After receipt-level discount
///     .TotalTaxAmount(51.45m)   // Total tax
///     .TotalAmount(296.45m)     // Final amount
///     
///     // Simple product addition (POS provides all amounts)
///     .AddProduct("SKU123", "Nike", "AirMax", "125.00", "250.00", 21.0, "52.50", "302.50", 2.0)
///     
///     // Or use full Product builder for complex cases
///     .AddProduct(Product.Builder()
///         .Name("Laptop")
///         .Quantity(1.0)
///         .UnitCode(UnitCode.ONE)
///         .UnitPrice("100.00")
///         .Subtotal("100.00")
///         .Total("121.00")
///         .AddTax(21.0, "VAT", "21.00")
///         .Build())
///     
///     // Receipt-level adjustments
///     .AddDiscount("5.00", "Loyalty discount")
///     .AddCharge("3.00", "Service fee")
///     
///     // Tax breakdown
///     .AddTax(Tax.Builder()
///         .Rate(21.0)
///         .Type("VAT")
///         .Amount("19.95")
///         .Label("VAT 21%")
///         .Build())
///     
///     // Optional fields
///     .Note("Thank you for your purchase!")
///     .Build();
/// </code>
///
/// <para><b>Mandatory Fields:</b></para>
/// <list type="bullet">
///   <item><b>documentNumber</b>: Unique receipt/invoice identifier</item>
///   <item><b>issueDate</b>: Receipt issue date</item>
///   <item><b>currency</b>: ISO 4217 currency code (e.g., "EUR", "USD")</item>
///   <item><b>invoiceSubtotal</b>: Sum of all product line totals</item>
///   <item><b>totalBeforeTax</b>: Total excluding taxes (after receipt-level adjustments)</item>
///   <item><b>totalTaxAmount</b>: Total tax amount (POS-calculated)</item>
///   <item><b>totalAmount</b>: Final amount charged to customer</item>
///   <item><b>products</b>: List of products (minimum 1 required)</item>
///   <item><b>taxes</b>: Tax breakdown by category/rate</item>
/// </list>
///
/// <para><b>Optional Fields:</b></para>
/// <list type="bullet">
///   <item><b>identifiers</b>: Additional identifiers (order number, PO number, etc.)</item>
///   <item><b>transactionDate</b>: When transaction was initiated</item>
///   <item><b>purchaseDate</b>: When purchase was made</item>
///   <item><b>discounts</b>: Receipt-level discounts</item>
///   <item><b>charges</b>: Receipt-level charges (delivery, service fees, etc.)</item>
///   <item><b>period</b>: Billing/service period (for subscriptions, rentals)</item>
///   <item><b>note</b>: Additional seller notes</item>
/// </list>
///
/// <para><b>Key Differences from invoiceSubtotal and totalBeforeTax:</b></para>
/// <list type="bullet">
///   <item><b>invoiceSubtotal</b>: Sum of all product line totals (before receipt-level adjustments)</item>
///   <item><b>totalBeforeTax</b>: invoiceSubtotal ± receipt-level discounts/charges</item>
///   <item>Example: invoiceSubtotal = $100, discount = $5, charge = $3 → totalBeforeTax = $98</item>
/// </list>
///
/// <para><b>Validation Rules:</b></para>
/// <list type="bullet">
///   <item>All mandatory fields must be present</item>
///   <item>At least one product is required</item>
///   <item>Currency must be a valid ISO 4217 code</item>
///   <item>All monetary amounts must be non-negative</item>
///   <item>Products must have valid unit codes</item>
/// </list>
/// </summary>
public class ReceiptTemplateRequest
{
    // ===== MANDATORY FIELDS =====

    /// <summary>
    /// A unique document identifier provided by the seller (for example invoice number).
    /// </summary>
    [JsonPropertyName("documentNumber")]
    public string documentNumber { get; }

    /// <summary>
    /// A list of identifiers for the receipt. For example order number, purchase order number, etc.
    /// </summary>
    [JsonPropertyName("identifiers")]
    public IReadOnlyList<Identifier> identifiers { get; }

    /// <summary>
    /// The date when the purchase receipt was issued.
    /// </summary>
    [JsonPropertyName("issueDate")]
    public DateTimeOffset issueDate { get; }

    /// <summary>
    /// The currency in which the purchase receipt is issued.
    /// Must be a valid ISO 4217 currency code (e.g., "EUR", "USD").
    /// </summary>
    [JsonPropertyName("currency")]
    public string currency { get; }

    /// <summary>
    /// The subtotal of the invoice, calculated as the sum of all line items before any overall
    /// invoice-level adjustments or taxes.
    /// This amount represents the total value of goods or services listed on the invoice.
    /// </summary>
    [JsonPropertyName("invoiceSubtotal")]
    public decimal invoiceSubtotal { get; }

    /// <summary>
    /// The total amount of the invoice excluding taxes, but including any overall discounts or charges.
    /// This amount may differ from the invoiceSubtotal if there are invoice-level adjustments.
    /// </summary>
    [JsonPropertyName("totalBeforeTax")]
    public decimal totalBeforeTax { get; }

    /// <summary>
    /// The total tax amount for the receipt.
    /// This should equal the sum of all tax amounts in the taxes list.
    /// </summary>
    [JsonPropertyName("totalTaxAmount")]
    public decimal totalTaxAmount { get; }

    /// <summary>
    /// The total amount charged to the customer.
    /// Should equal totalBeforeTax + totalTaxAmount.
    /// </summary>
    [JsonPropertyName("totalAmount")]
    public decimal totalAmount { get; }

    /// <summary>
    /// The list of products included in the purchase receipt.
    /// Must contain at least one product.
    /// </summary>
    [JsonPropertyName("products")]
    public IReadOnlyList<Product> products { get; }

    /// <summary>
    /// Overall discounts applied at the receipt level (not product-specific).
    /// Examples: "Loyalty discount", "Coupon code", "Volume discount"
    /// </summary>
    [JsonPropertyName("discounts")]
    public IReadOnlyList<Discount> discounts { get; }

    /// <summary>
    /// Overall charges applied at the receipt level (not product-specific).
    /// Examples: "Delivery fee", "Service charge", "Payment processing fee"
    /// </summary>
    [JsonPropertyName("charges")]
    public IReadOnlyList<Charge> charges { get; }

    /// <summary>
    /// Tax breakdown for the receipt.
    /// Each Tax entry represents a tax category applied to the receipt (e.g., VAT 21%, VAT 9%).
    /// If no taxes are applied, then you need a tax category with 0% rate.
    /// The total tax amount is the sum of all tax amounts in this list.
    /// </summary>
    [JsonPropertyName("taxes")]
    public IReadOnlyList<Tax> taxes { get; }

    // ===== OPTIONAL FIELDS =====

    /// <summary>
    /// The date when the purchase transaction was initiated.
    /// Only needed when different from the issue date.
    /// </summary>
    [JsonPropertyName("transactionDate")]
    public DateTimeOffset? transactionDate { get; }

    /// <summary>
    /// The date when the purchase was made.
    /// Only needed when different from the issue date.
    /// </summary>
    [JsonPropertyName("purchaseDate")]
    public DateTimeOffset? purchaseDate { get; }

    /// <summary>
    /// The billing or service period covered by this receipt.
    /// Use this when the purchase covers a specific time range rather than a single point in time.
    /// 
    /// <para>Common use cases:</para>
    /// <list type="bullet">
    ///   <item>Subscription services (e.g., monthly SaaS subscription from Jan 1 - Jan 31)</item>
    ///   <item>Rental periods (e.g., car rental from Dec 1 - Dec 7)</item>
    ///   <item>Utility bills (e.g., electricity usage from Nov 1 - Nov 30)</item>
    ///   <item>Service contracts (e.g., maintenance contract from Q1 2024)</item>
    /// </list>
    /// 
    /// <para>Leave empty for one-time purchases or instant transactions.</para>
    /// </summary>
    [JsonPropertyName("period")]
    public Period? period { get; }

    /// <summary>
    /// A note that the seller wants to include on the purchase receipt.
    /// </summary>
    [JsonPropertyName("note")]
    public string? note { get; }

    // ===== CONSTRUCTOR =====

    private ReceiptTemplateRequest(
        string documentNumber,
        List<Identifier>? identifiers,
        DateTimeOffset issueDate,
        string currency,
        decimal invoiceSubtotal,
        decimal totalBeforeTax,
        decimal totalTaxAmount,
        decimal totalAmount,
        List<Product>? products,
        List<Discount>? discounts,
        List<Charge>? charges,
        List<Tax>? taxes,
        DateTimeOffset? transactionDate,
        DateTimeOffset? purchaseDate,
        Period? period,
        string? note)
    {
        this.documentNumber = documentNumber;
        this.identifiers = identifiers?.AsReadOnly() ?? new List<Identifier>().AsReadOnly();
        this.issueDate = issueDate;
        this.currency = currency;
        this.invoiceSubtotal = invoiceSubtotal;
        this.totalBeforeTax = totalBeforeTax;
        this.totalTaxAmount = totalTaxAmount;
        this.totalAmount = totalAmount;
        this.products = products?.AsReadOnly() ?? new List<Product>().AsReadOnly();
        this.discounts = discounts?.AsReadOnly() ?? new List<Discount>().AsReadOnly();
        this.charges = charges?.AsReadOnly() ?? new List<Charge>().AsReadOnly();
        this.taxes = taxes?.AsReadOnly() ?? new List<Tax>().AsReadOnly();
        this.transactionDate = transactionDate;
        this.purchaseDate = purchaseDate;
        this.period = period;
        this.note = note;
    }

    // ===== BUILDER =====

    public static ReceiptTemplateRequestBuilder Builder() => new ReceiptTemplateRequestBuilder();

    public sealed class ReceiptTemplateRequestBuilder
    {
        private string? _documentNumber;
        private List<Identifier>? _identifiers;
        private DateTimeOffset? _issueDate;
        private string? _currency;
        private decimal? _invoiceSubtotal;
        private decimal? _totalBeforeTax;
        private decimal? _totalTaxAmount;
        private decimal? _totalAmount;
        private List<Product>? _products;
        private List<Discount>? _discounts;
        private List<Charge>? _charges;
        private List<Tax>? _taxes;
        private DateTimeOffset? _transactionDate;
        private DateTimeOffset? _purchaseDate;
        private Period? _period;
        private string? _note;

        internal ReceiptTemplateRequestBuilder() { }

        public ReceiptTemplateRequestBuilder From(ReceiptTemplateRequest other)
        {
            _documentNumber = other.documentNumber;
            _identifiers = other.identifiers.ToList();
            _issueDate = other.issueDate;
            _currency = other.currency;
            _invoiceSubtotal = other.invoiceSubtotal;
            _totalBeforeTax = other.totalBeforeTax;
            _totalTaxAmount = other.totalTaxAmount;
            _totalAmount = other.totalAmount;
            _products = other.products.ToList();
            _discounts = other.discounts.ToList();
            _charges = other.charges.ToList();
            _taxes = other.taxes.ToList();
            _transactionDate = other.transactionDate;
            _purchaseDate = other.purchaseDate;
            _period = other.period;
            _note = other.note;
            return this;
        }

        public ReceiptTemplateRequestBuilder DocumentNumber(string documentNumber)
        {
            _documentNumber = documentNumber;
            return this;
        }

        public ReceiptTemplateRequestBuilder Identifiers(List<Identifier> identifiers)
        {
            _identifiers = identifiers;
            return this;
        }

        public ReceiptTemplateRequestBuilder IssueDate(DateTimeOffset issueDate)
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
        public ReceiptTemplateRequestBuilder Currency(string currency)
        {
            if (currency != null)
            {
                try
                {
                    // Validate using .NET's RegionInfo to check ISO 4217 codes
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

        public ReceiptTemplateRequestBuilder InvoiceSubtotal(decimal invoiceSubtotal)
        {
            _invoiceSubtotal = invoiceSubtotal;
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

        public ReceiptTemplateRequestBuilder Products(List<Product> products)
        {
            _products = products;
            return this;
        }

        /// <summary>
        /// Convenience method to add a single product to the receipt.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddProduct(Product product)
        {
            _products ??= new List<Product>();
            _products.Add(product);
            return this;
        }

        /// <summary>
        /// Convenience method to add a simple product with basic details.
        /// All amounts must be pre-calculated by the POS system.
        /// </summary>
        /// <param name="identifier">Product identifier/SKU</param>
        /// <param name="brand">Product brand/manufacturer</param>
        /// <param name="name">Product name/model</param>
        /// <param name="unitPrice">Price per unit</param>
        /// <param name="subtotal">Subtotal (unitPrice × quantity, pre-calculated)</param>
        /// <param name="taxRate">Tax rate as percentage (e.g., 21.0 for 21%)</param>
        /// <param name="taxAmount">Tax amount (pre-calculated)</param>
        /// <param name="total">Total amount including tax (pre-calculated)</param>
        /// <param name="quantity">Number of units</param>
        /// <returns>This builder</returns>
        public ReceiptTemplateRequestBuilder AddProduct(string identifier, string brand, string name, string unitPrice, string subtotal, double taxRate, string taxAmount, string total, double quantity)
        {
            return AddProduct(Product.Builder()
                .Brand(brand)
                .Name(name)
                .Identifier(identifier)
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
        public ReceiptTemplateRequestBuilder AddProduct(string identifier, string brand, string name, decimal unitPrice, decimal subtotal, double taxRate, decimal taxAmount, decimal total, double quantity)
        {
            return AddProduct(identifier, brand, name, unitPrice.ToString(), subtotal.ToString(), taxRate, taxAmount.ToString(), total.ToString(), quantity);
        }

        public ReceiptTemplateRequestBuilder Discounts(List<Discount> discounts)
        {
            _discounts = discounts;
            return this;
        }

        /// <summary>
        /// Convenience method to add a single discount to the receipt.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddDiscount(Discount discount)
        {
            _discounts ??= new List<Discount>();
            _discounts.Add(discount);
            return this;
        }

        /// <summary>
        /// Convenience method to add a discount with amount and label.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddDiscount(decimal amount, string label)
        {
            return AddDiscount(Discount.Of(amount, label));
        }

        /// <summary>
        /// Convenience method to add a discount with amount (string) and label.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddDiscount(string amount, string label)
        {
            return AddDiscount(Discount.Of(decimal.Parse(amount), label));
        }

        /// <summary>
        /// Convenience method to add a discount with amount, percentage, and label.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddDiscount(decimal amount, double percentage, string label)
        {
            return AddDiscount(Discount.Of(amount, percentage, label));
        }

        /// <summary>
        /// Convenience method to add a discount with amount (string), percentage, and label.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddDiscount(string amount, double percentage, string label)
        {
            return AddDiscount(Discount.Of(decimal.Parse(amount), percentage, label));
        }

        public ReceiptTemplateRequestBuilder Charges(List<Charge> charges)
        {
            _charges = charges;
            return this;
        }

        /// <summary>
        /// Convenience method to add a single charge to the receipt.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddCharge(Charge charge)
        {
            _charges ??= new List<Charge>();
            _charges.Add(charge);
            return this;
        }

        /// <summary>
        /// Convenience method to add a charge with amount and label.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddCharge(decimal amount, string label)
        {
            return AddCharge(Charge.Of(amount, label));
        }

        /// <summary>
        /// Convenience method to add a charge with amount (string) and label.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddCharge(string amount, string label)
        {
            return AddCharge(Charge.Of(decimal.Parse(amount), label));
        }

        /// <summary>
        /// Convenience method to add a charge with amount, percentage, and label.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddCharge(decimal amount, double percentage, string label)
        {
            return AddCharge(Charge.Of(amount, percentage, label));
        }

        /// <summary>
        /// Convenience method to add a charge with amount (string), percentage, and label.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddCharge(string amount, double percentage, string label)
        {
            return AddCharge(Charge.Of(decimal.Parse(amount), percentage, label));
        }

        public ReceiptTemplateRequestBuilder Taxes(List<Tax> taxes)
        {
            _taxes = taxes;
            return this;
        }

        /// <summary>
        /// Convenience method to add a single tax entry.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddTax(Tax tax)
        {
            _taxes ??= new List<Tax>();
            _taxes.Add(tax);
            return this;
        }

        /// <summary>
        /// Convenience method to add a tax with rate, type, and amount.
        /// </summary>
        public ReceiptTemplateRequestBuilder AddTax(double rate, string type, decimal amount)
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

        public ReceiptTemplateRequestBuilder Period(Period? period)
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
            if (_invoiceSubtotal == null)
                throw new InvalidOperationException("InvoiceSubtotal is required");
            if (_totalBeforeTax == null)
                throw new InvalidOperationException("TotalBeforeTax is required");
            if (_totalTaxAmount == null)
                throw new InvalidOperationException("TotalTaxAmount is required");
            if (_totalAmount == null)
                throw new InvalidOperationException("TotalAmount is required");

            return new ReceiptTemplateRequest(
                _documentNumber,
                _identifiers,
                _issueDate.Value,
                _currency,
                _invoiceSubtotal.Value,
                _totalBeforeTax.Value,
                _totalTaxAmount.Value,
                _totalAmount.Value,
                _products,
                _discounts,
                _charges,
                _taxes,
                _transactionDate,
                _purchaseDate,
                _period,
                _note
            );
        }
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not ReceiptTemplateRequest other) return false;
        return documentNumber == other.documentNumber
            && identifiers.SequenceEqual(other.identifiers)
            && issueDate == other.issueDate
            && currency == other.currency
            && invoiceSubtotal == other.invoiceSubtotal
            && totalBeforeTax == other.totalBeforeTax
            && totalTaxAmount == other.totalTaxAmount
            && totalAmount == other.totalAmount
            && products.SequenceEqual(other.products)
            && discounts.SequenceEqual(other.discounts)
            && charges.SequenceEqual(other.charges)
            && taxes.SequenceEqual(other.taxes)
            && transactionDate == other.transactionDate
            && purchaseDate == other.purchaseDate
            && Equals(period, other.period)
            && note == other.note;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(documentNumber);
        hash.Add(issueDate);
        hash.Add(currency);
        hash.Add(invoiceSubtotal);
        hash.Add(totalBeforeTax);
        hash.Add(totalTaxAmount);
        hash.Add(totalAmount);
        hash.Add(transactionDate);
        hash.Add(purchaseDate);
        hash.Add(period);
        hash.Add(note);
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        return $"ReceiptTemplateRequest{{" +
               $"documentNumber='{documentNumber}', " +
               $"identifiers={identifiers.Count} items, " +
               $"issueDate={issueDate}, " +
               $"currency='{currency}', " +
               $"invoiceSubtotal={invoiceSubtotal}, " +
               $"totalBeforeTax={totalBeforeTax}, " +
               $"totalTaxAmount={totalTaxAmount}, " +
               $"totalAmount={totalAmount}, " +
               $"products={products.Count} items, " +
               $"discounts={discounts.Count} items, " +
               $"charges={charges.Count} items, " +
               $"taxes={taxes.Count} items, " +
               $"transactionDate={transactionDate}, " +
               $"purchaseDate={purchaseDate}, " +
               $"period={period}, " +
               $"note={note}}}";
    }
}
