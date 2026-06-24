using GeneratedModels = Cheqi.Sdk.Models.Generated;
using BarcodeType = Cheqi.Sdk.Models.Generated.BarcodeType;

namespace Cheqi.Sdk.Models;

/// <summary>
/// A barcode or QR code to embed in a receipt or product line.
/// Extends the generated base with factory methods and <see cref="BarcodeType"/> support.
///
/// <para><b>Example:</b></para>
/// <code>
/// var returnCode = Barcode.QrCode("https://example.com/return/12345", "Return code");
/// var loyaltyCode = Barcode.Of(BarcodeType.CODE128, "LOYALTY-ABC-123", "Loyalty points");
/// </code>
/// </summary>
public sealed class Barcode : GeneratedModels.Barcode
{
    public Barcode() { }

    // ===== FACTORY METHODS =====

    public static Barcode Of(BarcodeType type, string data, string? label = null)
    {
        var b = new Barcode();
        b.Type = GeneratedModels.BarcodeTypeValueConverter.ToJsonValue(type);
        b.Data = data;
        b.Label = label;
        return b;
    }

    public static Barcode QrCode(string data, string? label = null)
    {
        return Of(BarcodeType.QRCODE, data, label);
    }

    // ===== BUILDER =====

    public static BarcodeBuilder Builder() => new BarcodeBuilder();

    public sealed class BarcodeBuilder
    {
        private BarcodeType? _type;
        private string? _data;
        private string? _label;

        internal BarcodeBuilder() { }

        public BarcodeBuilder Type(BarcodeType type)
        {
            _type = type;
            return this;
        }

        public BarcodeBuilder Data(string data)
        {
            _data = data;
            return this;
        }

        public BarcodeBuilder Label(string? label)
        {
            _label = label;
            return this;
        }

        public Barcode Build()
        {
            if (_type == null)
                throw new InvalidOperationException("Barcode type is required");
            if (string.IsNullOrEmpty(_data))
                throw new InvalidOperationException("Barcode data is required");

            return Of(_type.Value, _data!, _label);
        }
    }
}
