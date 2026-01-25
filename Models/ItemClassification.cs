using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Item classification DTO representing product classification information.
///
/// <para><b>Mandatory Fields:</b></para>
/// <list type="bullet">
///   <item><b>listId</b>: The classification scheme identifier</item>
///   <item><b>value</b>: The classification code/value within the scheme</item>
/// </list>
///
/// <para><b>Common Classification Schemes:</b></para>
/// <list type="bullet">
///   <item><b>EAN</b>: European Article Number (barcode standard)</item>
///   <item><b>GTIN</b>: Global Trade Item Number (includes EAN, UPC, etc.)</item>
///   <item><b>UPC</b>: Universal Product Code</item>
///   <item><b>UNSPSC</b>: United Nations Standard Products and Services Code</item>
///   <item><b>CPV</b>: Common Procurement Vocabulary</item>
///   <item><b>CAS</b>: Chemical Abstracts Service Registry Number</item>
///   <item><b>ISBN</b>: International Standard Book Number</item>
///   <item><b>ISSN</b>: International Standard Serial Number</item>
/// </list>
///
/// <para><b>Examples:</b></para>
/// <list type="bullet">
///   <item>EAN: listId="EAN", value="1234567890123"</item>
///   <item>GTIN: listId="GTIN", value="01234567890128"</item>
///   <item>UNSPSC: listId="UNSPSC", value="12345678"</item>
/// </list>
/// </summary>
public sealed class ItemClassification
{
    // ===== MANDATORY FIELDS =====

    /// <summary>
    /// The classification scheme identifier.
    /// Examples: "UNSPSC", "CPV", "GTIN", "CAS"
    /// </summary>
    [JsonPropertyName("listId")]
    public string listId { get; }

    /// <summary>
    /// The classification code/value within the specified scheme.
    /// The format depends on the classification scheme used.
    /// </summary>
    [JsonPropertyName("value")]
    public string value { get; }

    // ===== CONSTRUCTOR =====

    private ItemClassification(string listId, string value)
    {
        this.listId = listId;
        this.value = value;
    }

    // ===== BUILDER =====

    public static ItemClassificationBuilder Builder() => new ItemClassificationBuilder();

    public sealed class ItemClassificationBuilder
    {
        private string? _listId;
        private string? _value;

        internal ItemClassificationBuilder() { }

        public ItemClassificationBuilder From(ItemClassification other)
        {
            _listId = other.listId;
            _value = other.value;
            return this;
        }

        public ItemClassificationBuilder ListId(string listId)
        {
            _listId = listId;
            return this;
        }

        public ItemClassificationBuilder Value(string value)
        {
            _value = value;
            return this;
        }

        public ItemClassification Build()
        {
            if (_listId == null)
                throw new InvalidOperationException("ListId is required");
            if (_value == null)
                throw new InvalidOperationException("Value is required");

            return new ItemClassification(_listId, _value);
        }
    }

    // ===== VALIDATION METHODS =====

    /// <summary>
    /// Validates this item classification.
    /// </summary>
    /// <returns>true if all mandatory fields are present and valid</returns>
    public bool IsValid() => GetValidationErrors().Count == 0;

    /// <summary>
    /// Gets detailed validation errors for this item classification.
    /// </summary>
    /// <returns>List of validation error messages, empty if valid</returns>
    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(listId))
        {
            errors.Add("List ID is required (classification scheme identifier, e.g., 'GTIN', 'UNSPSC')");
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add("Value is required (classification code within the specified scheme)");
        }

        // Basic format validation for common schemes
        if (!string.IsNullOrWhiteSpace(listId) && !string.IsNullOrWhiteSpace(value))
        {
            var scheme = listId.Trim().ToUpperInvariant();
            var code = value.Trim();

            switch (scheme)
            {
                case "EAN":
                    if (!Regex.IsMatch(code, @"^\d{13}$"))
                        errors.Add("EAN code must be exactly 13 digits");
                    break;
                case "UPC":
                    if (!Regex.IsMatch(code, @"^\d{12}$"))
                        errors.Add("UPC code must be exactly 12 digits");
                    break;
                case "GTIN":
                    if (!Regex.IsMatch(code, @"^(\d{8}|\d{12}|\d{13}|\d{14})$"))
                        errors.Add("GTIN code must be 8, 12, 13, or 14 digits");
                    break;
                case "UNSPSC":
                    if (!Regex.IsMatch(code, @"^\d{8}$"))
                        errors.Add("UNSPSC code must be exactly 8 digits");
                    break;
                case "ISBN":
                    if (!Regex.IsMatch(code, @"^(\d{10}|\d{13})$"))
                        errors.Add("ISBN must be 10 or 13 digits");
                    break;
            }
        }

        return errors;
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not ItemClassification other) return false;
        return listId == other.listId && value == other.value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(listId, value);
    }

    public override string ToString()
    {
        return $"ItemClassification{{listId='{listId}', value='{value}'}}";
    }
}
