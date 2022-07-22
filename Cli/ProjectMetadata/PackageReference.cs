using System.Diagnostics;
using System.Xml.Serialization;

namespace YadaYada.BuildTools.Cli.ProjectMetadata;

[XmlRoot(elementName: "PackageReference")]
[DebuggerDisplay("{Include}")]
public class PackageReference
{
    [XmlAttribute(attributeName: "Include")]
    public string Include { get; set; }

}