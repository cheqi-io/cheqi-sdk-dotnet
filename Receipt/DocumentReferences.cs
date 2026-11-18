using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.Receipt;

internal static class DocumentReferences
{
    internal static string Hash(string documentNumber)
    {
        if (string.IsNullOrWhiteSpace(documentNumber)) throw new ArgumentException("documentNumber is required");
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("cheqi:issuer-document:v1:" + documentNumber))).ToLowerInvariant();
    }
    internal static string AddDownloadRecipient(ReceiptEnvelope envelope)
    {
        using var key = RSA.Create(2048);
        envelope.Documents["DOWNLOAD_RECIPIENT_KEY"] = new ReceiptEnvelopeDocument(JsonSerializer.Serialize(new {
            algorithm = "RSA-OAEP-256", privateKey = Convert.ToBase64String(key.ExportPkcs8PrivateKey())
        }), ReceiptEnvelopeDocument.MediaTypeEnum.ApplicationJson);
        return Convert.ToBase64String(key.ExportSubjectPublicKeyInfo());
    }
}
