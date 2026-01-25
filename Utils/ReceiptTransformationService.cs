using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Models.Ubl;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Utils;

/// <summary>
/// Utility class for transforming receipt data between JSON and XML formats.
/// 
/// This utility provides static methods to convert JSON string representations
/// of PurchaseReceipt objects into UBL-compliant XML format using XML serialization.
/// 
/// Example usage:
/// <code>
/// string jsonReceipt = "{\"id\":{\"value\":\"RCP-001\"},...}";
/// string xmlReceipt = ReceiptTransformationService.TransformJsonToXml(jsonReceipt);
/// </code>
/// </summary>
public static class ReceiptTransformationService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Transforms a JSON string representation of a PurchaseReceipt into UBL-compliant XML.
    /// </summary>
    /// <param name="jsonReceipt">JSON string representation of a PurchaseReceipt</param>
    /// <returns>UBL-compliant XML string</returns>
    /// <exception cref="CheqiSDKException">if JSON parsing or XML marshalling fails</exception>
    public static string TransformJsonToXml(string jsonReceipt)
    {
        if (string.IsNullOrWhiteSpace(jsonReceipt))
        {
            throw new CheqiSDKException("JSON receipt string cannot be null or empty");
        }

        try
        {
            var receipt = JsonSerializer.Deserialize<PurchaseReceipt>(jsonReceipt, JsonOptions);
            if (receipt == null)
            {
                throw new CheqiSDKException("Failed to deserialize JSON to PurchaseReceipt");
            }
            return TransformReceiptToXml(receipt);
        }
        catch (CheqiSDKException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new CheqiSDKException("Failed to transform JSON to XML: " + e.Message, e);
        }
    }

    /// <summary>
    /// Transforms a PurchaseReceipt object into UBL-compliant XML.
    /// </summary>
    /// <param name="receipt">PurchaseReceipt object to transform</param>
    /// <returns>UBL-compliant XML string</returns>
    /// <exception cref="CheqiSDKException">if XML marshalling fails</exception>
    public static string TransformReceiptToXml(PurchaseReceipt receipt)
    {
        if (receipt == null)
        {
            throw new CheqiSDKException("PurchaseReceipt cannot be null");
        }

        try
        {
            var serializer = new XmlSerializer(typeof(PurchaseReceipt));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = System.Text.Encoding.UTF8,
                OmitXmlDeclaration = false
            };

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);
            serializer.Serialize(xmlWriter, receipt);
            return stringWriter.ToString();
        }
        catch (Exception e)
        {
            throw new CheqiSDKException("Failed to marshal PurchaseReceipt to XML: " + e.Message, e);
        }
    }

    /// <summary>
    /// Parses a JSON string into a PurchaseReceipt object.
    /// </summary>
    /// <param name="jsonReceipt">JSON string representation of a PurchaseReceipt</param>
    /// <returns>PurchaseReceipt object</returns>
    /// <exception cref="CheqiSDKException">if JSON parsing fails</exception>
    public static PurchaseReceipt ParseJsonToReceipt(string jsonReceipt)
    {
        if (string.IsNullOrWhiteSpace(jsonReceipt))
        {
            throw new CheqiSDKException("JSON receipt string cannot be null or empty");
        }

        try
        {
            var receipt = JsonSerializer.Deserialize<PurchaseReceipt>(jsonReceipt, JsonOptions);
            if (receipt == null)
            {
                throw new CheqiSDKException("Failed to deserialize JSON to PurchaseReceipt");
            }
            return receipt;
        }
        catch (CheqiSDKException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new CheqiSDKException("Failed to parse JSON to PurchaseReceipt: " + e.Message, e);
        }
    }

    /// <summary>
    /// Parses XML string into JSON representation.
    /// </summary>
    /// <param name="xmlReceipt">XML string representation of a PurchaseReceipt</param>
    /// <returns>JSON string representation</returns>
    /// <exception cref="CheqiSDKException">if XML parsing fails</exception>
    public static string ParseXmlToJson(string xmlReceipt)
    {
        if (string.IsNullOrWhiteSpace(xmlReceipt))
        {
            throw new CheqiSDKException("XML receipt string cannot be null or empty");
        }

        try
        {
            var serializer = new XmlSerializer(typeof(PurchaseReceipt));
            using var stringReader = new StringReader(xmlReceipt);
            var receipt = serializer.Deserialize(stringReader) as PurchaseReceipt;
            
            if (receipt == null)
            {
                throw new CheqiSDKException("Failed to deserialize XML to PurchaseReceipt");
            }

            return JsonSerializer.Serialize(receipt, JsonOptions);
        }
        catch (CheqiSDKException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new CheqiSDKException("Failed to parse XML to JSON: " + e.Message, e);
        }
    }

    /// <summary>
    /// Serializes a PurchaseReceipt object to JSON string.
    /// </summary>
    /// <param name="receipt">PurchaseReceipt object to serialize</param>
    /// <returns>JSON string representation</returns>
    /// <exception cref="CheqiSDKException">if JSON serialization fails</exception>
    public static string SerializeReceiptToJson(PurchaseReceipt receipt)
    {
        if (receipt == null)
        {
            throw new CheqiSDKException("PurchaseReceipt cannot be null");
        }

        try
        {
            return JsonSerializer.Serialize(receipt, JsonOptions);
        }
        catch (Exception e)
        {
            throw new CheqiSDKException("Failed to serialize PurchaseReceipt to JSON: " + e.Message, e);
        }
    }
}
