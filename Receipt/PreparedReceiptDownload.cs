using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.Receipt;

/// <summary>Persist this encrypted request and its secret URL before displaying the QR code.</summary>
public sealed class PreparedReceiptDownload
{
    private readonly ClientReceiptDownloadRequest _uploadRequest;
    public string DownloadUrl { get; }
    public ClientReceiptDownloadRequest UploadRequest => Copy(_uploadRequest);
    [JsonIgnore] public string DownloadId => _uploadRequest.DownloadId;
    [JsonIgnore] public string Ciphertext => _uploadRequest.Ciphertext;
    [JsonIgnore] public string? TemplateHash => _uploadRequest.TemplateHash;

    [JsonConstructor]
    public PreparedReceiptDownload(string downloadUrl, ClientReceiptDownloadRequest uploadRequest)
    {
        DownloadUrl = downloadUrl ?? throw new ArgumentNullException(nameof(downloadUrl));
        ArgumentNullException.ThrowIfNull(uploadRequest);
        ArgumentNullException.ThrowIfNull(uploadRequest.DownloadId);
        ArgumentNullException.ThrowIfNull(uploadRequest.Ciphertext);
        _uploadRequest = Copy(uploadRequest);
    }
    private static ClientReceiptDownloadRequest Copy(ClientReceiptDownloadRequest request)
    {
        var copy = new ClientReceiptDownloadRequest(request.DownloadId, request.Ciphertext);
        if (request.TemplateHash != null) copy.TemplateHash = request.TemplateHash;
        return copy;
    }
    public override string ToString() => "PreparedReceiptDownload { Url = <redacted>, Ciphertext = <redacted> }";
}
