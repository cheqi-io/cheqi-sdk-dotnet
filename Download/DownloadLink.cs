namespace Cheqi.Sdk.Download;

/// <summary>Client-generated download credentials. The URL fragment is a secret AES key.</summary>
public sealed class DownloadLink
{
    public string DownloadId { get; }
    public string ContentKey { get; }
    public string Url { get; }
    public DownloadLink(string downloadId, string contentKey, string url)
    {
        DownloadId = downloadId ?? throw new ArgumentNullException(nameof(downloadId));
        ContentKey = contentKey ?? throw new ArgumentNullException(nameof(contentKey));
        Url = url ?? throw new ArgumentNullException(nameof(url));
    }
    public override string ToString() => $"DownloadLink {{ DownloadId = {DownloadId}, Url = <redacted> }}";
}
