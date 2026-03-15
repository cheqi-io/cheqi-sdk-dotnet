using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace Cheqi.Sdk.Utils;

/// <summary>
/// Canonicalizes XML using exclusive C14N after removing formatting-only whitespace.
/// </summary>
public sealed class XmlCanonicalService
{
    public string Canonicalize(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new ArgumentException("XML cannot be null or empty", nameof(xml));
        }

        var document = new XmlDocument
        {
            PreserveWhitespace = true
        };
        document.LoadXml(xml);
        RemoveWhitespaceOnlyTextNodes(document);

        var transform = new XmlDsigExcC14NTransform(includeComments: false);
        transform.LoadInput(document);

        using var stream = (Stream)transform.GetOutput(typeof(Stream));
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    public string CalculateHash(string xml)
    {
        var canonical = Canonicalize(xml);
        return HashUtils.Sha256Hex(canonical);
    }

    private static void RemoveWhitespaceOnlyTextNodes(XmlNode node)
    {
        for (var i = node.ChildNodes.Count - 1; i >= 0; i--)
        {
            var child = node.ChildNodes[i];
            if ((child.NodeType == XmlNodeType.Text
                    || child.NodeType == XmlNodeType.Whitespace
                    || child.NodeType == XmlNodeType.SignificantWhitespace)
                && string.IsNullOrWhiteSpace(child.InnerText))
            {
                node.RemoveChild(child);
                continue;
            }

            RemoveWhitespaceOnlyTextNodes(child);
        }
    }
}
