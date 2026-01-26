using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Item - represents a product or service item.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Item
{
    /// <summary>
    /// Item description.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public string? Description { get; set; }

    /// <summary>
    /// Pack quantity.
    /// </summary>
    [JsonPropertyName("packQuantity")]
    [XmlElement("PackQuantity", Namespace = UblNamespaces.Cbc)]
    public Quantity? PackQuantity { get; set; }

    /// <summary>
    /// Pack size numeric.
    /// </summary>
    [JsonPropertyName("packSizeNumeric")]
    [XmlElement("PackSizeNumeric", Namespace = UblNamespaces.Cbc)]
    public double? PackSizeNumeric { get; set; }

    /// <summary>
    /// Catalogue indicator.
    /// </summary>
    [JsonPropertyName("catalogueIndicator")]
    [XmlElement("CatalogueIndicator", Namespace = UblNamespaces.Cbc)]
    public bool? CatalogueIndicator { get; set; }

    /// <summary>
    /// Item name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public string? Name { get; set; }

    /// <summary>
    /// Item type code.
    /// </summary>
    [JsonPropertyName("itemTypeCode")]
    [XmlElement("ItemTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? ItemTypeCode { get; set; }

    /// <summary>
    /// Hazardous risk indicator.
    /// </summary>
    [JsonPropertyName("hazardousRiskIndicator")]
    [XmlElement("HazardousRiskIndicator", Namespace = UblNamespaces.Cbc)]
    public bool? HazardousRiskIndicator { get; set; }

    /// <summary>
    /// Additional information.
    /// </summary>
    [JsonPropertyName("additionalInformation")]
    [XmlElement("AdditionalInformation", Namespace = UblNamespaces.Cbc)]
    public List<string>? AdditionalInformation { get; set; }

    /// <summary>
    /// Keywords.
    /// </summary>
    [JsonPropertyName("keyword")]
    [XmlElement("Keyword", Namespace = UblNamespaces.Cbc)]
    public List<string>? Keyword { get; set; }

    /// <summary>
    /// Brand names.
    /// </summary>
    [JsonPropertyName("brandName")]
    [XmlElement("BrandName", Namespace = UblNamespaces.Cbc)]
    public HashSet<string>? BrandName { get; set; }

    /// <summary>
    /// Model names.
    /// </summary>
    [JsonPropertyName("modelName")]
    [XmlElement("ModelName", Namespace = UblNamespaces.Cbc)]
    public List<string>? ModelName { get; set; }

    /// <summary>
    /// Warranty information.
    /// </summary>
    [JsonPropertyName("warrantyInformation")]
    [XmlElement("WarrantyInformation", Namespace = UblNamespaces.Cbc)]
    public List<string>? WarrantyInformation { get; set; }

    /// <summary>
    /// Buyer's item identification.
    /// </summary>
    [JsonPropertyName("buyersItemIdentification")]
    [XmlElement("BuyersItemIdentification", Namespace = UblNamespaces.Cac)]
    public ItemIdentification? BuyersItemIdentification { get; set; }

    /// <summary>
    /// Seller's item identification.
    /// </summary>
    [JsonPropertyName("sellersItemIdentification")]
    [XmlElement("SellersItemIdentification", Namespace = UblNamespaces.Cac)]
    public ItemIdentification? SellersItemIdentification { get; set; }

    /// <summary>
    /// Manufacturer's item identification.
    /// </summary>
    [JsonPropertyName("manufacturersItemIdentification")]
    [XmlElement("ManufacturersItemIdentification", Namespace = UblNamespaces.Cac)]
    public ItemIdentification? ManufacturersItemIdentification { get; set; }

    /// <summary>
    /// Standard item identification.
    /// </summary>
    [JsonPropertyName("standardItemIdentification")]
    [XmlElement("StandardItemIdentification", Namespace = UblNamespaces.Cac)]
    public ItemIdentification? StandardItemIdentification { get; set; }

    /// <summary>
    /// Catalogue item identification.
    /// </summary>
    [JsonPropertyName("catalogueItemIdentification")]
    [XmlElement("CatalogueItemIdentification", Namespace = UblNamespaces.Cac)]
    public ItemIdentification? CatalogueItemIdentification { get; set; }

    /// <summary>
    /// Additional item identifications.
    /// </summary>
    [JsonPropertyName("additionalItemIdentification")]
    [XmlElement("AdditionalItemIdentification", Namespace = UblNamespaces.Cac)]
    public List<ItemIdentification>? AdditionalItemIdentification { get; set; }

    /// <summary>
    /// Catalogue document reference.
    /// </summary>
    [JsonPropertyName("catalogueDocumentReference")]
    [XmlElement("CatalogueDocumentReference", Namespace = UblNamespaces.Cac)]
    public DocumentReference? CatalogueDocumentReference { get; set; }

    /// <summary>
    /// Item specification document references.
    /// </summary>
    [JsonPropertyName("itemSpecificationDocumentReference")]
    [XmlElement("ItemSpecificationDocumentReference", Namespace = UblNamespaces.Cac)]
    public List<DocumentReference>? ItemSpecificationDocumentReference { get; set; }

    /// <summary>
    /// Origin country.
    /// </summary>
    [JsonPropertyName("originCountry")]
    [XmlElement("OriginCountry", Namespace = UblNamespaces.Cac)]
    public Country? OriginCountry { get; set; }

    /// <summary>
    /// Commodity classifications.
    /// </summary>
    [JsonPropertyName("commodityClassification")]
    [XmlElement("CommodityClassification", Namespace = UblNamespaces.Cac)]
    public List<CommodityClassification>? CommodityClassification { get; set; }

    /// <summary>
    /// Transaction conditions.
    /// </summary>
    [JsonPropertyName("transactionConditions")]
    [XmlElement("TransactionConditions", Namespace = UblNamespaces.Cac)]
    public List<TransactionConditions>? TransactionConditions { get; set; }

    /// <summary>
    /// Classified tax categories.
    /// </summary>
    [JsonPropertyName("classifiedTaxCategory")]
    [XmlElement("ClassifiedTaxCategory")]
    public List<TaxCategory>? ClassifiedTaxCategory { get; set; }

    /// <summary>
    /// Additional item properties.
    /// </summary>
    [JsonPropertyName("additionalItemProperty")]
    [XmlElement("AdditionalItemProperty", Namespace = UblNamespaces.Cac)]
    public List<ItemProperty>? AdditionalItemProperty { get; set; }

    /// <summary>
    /// Manufacturer parties.
    /// </summary>
    [JsonPropertyName("manufacturerParty")]
    [XmlElement("ManufacturerParty", Namespace = UblNamespaces.Cac)]
    public List<Party>? ManufacturerParty { get; set; }

    /// <summary>
    /// Information content provider party.
    /// </summary>
    [JsonPropertyName("informationContentProviderParty")]
    [XmlElement("InformationContentProviderParty", Namespace = UblNamespaces.Cac)]
    public Party? InformationContentProviderParty { get; set; }

    /// <summary>
    /// Origin addresses.
    /// </summary>
    [JsonPropertyName("originAddress")]
    [XmlElement("OriginAddress", Namespace = UblNamespaces.Cac)]
    public List<Address>? OriginAddress { get; set; }

    /// <summary>
    /// Item instances.
    /// </summary>
    [JsonPropertyName("itemInstance")]
    [XmlElement("ItemInstance", Namespace = UblNamespaces.Cac)]
    public List<ItemInstance>? ItemInstance { get; set; }

    /// <summary>
    /// Certificates.
    /// </summary>
    [JsonPropertyName("certificate")]
    [XmlElement("Certificate", Namespace = UblNamespaces.Cac)]
    public List<Certificate>? Certificate { get; set; }

    /// <summary>
    /// Dimensions.
    /// </summary>
    [JsonPropertyName("dimension")]
    [XmlElement("Dimension", Namespace = UblNamespaces.Cac)]
    public List<Dimension>? Dimension { get; set; }

    /// <summary>
    /// Environmental emissions.
    /// </summary>
    [JsonPropertyName("environmentalEmission")]
    [XmlElement("EnvironmentalEmission", Namespace = UblNamespaces.Cac)]
    public List<EnvironmentalEmission>? EnvironmentalEmission { get; set; }
}
