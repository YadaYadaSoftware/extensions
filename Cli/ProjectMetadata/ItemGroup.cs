using System.Xml.Serialization;

namespace YadaYada.BuildTools.Cli.ProjectMetadata;

[XmlRoot(elementName: "ItemGroup")]
public class ItemGroup
{
    [XmlElement(elementName: "PackageReference")]
    public List<PackageReference> PackageReferences { get; set; } = new List<PackageReference>();

    [XmlElement(elementName: "ProjectReference")]
    public List<ProjectReference> ProjectReferences { get; set; } = new List<ProjectReference>();

    [XmlElement(elementName: "PackageId")]
    public string PackageId { get; set; }

    [XmlElement(elementName: "RootNamespace")]
    public string RootNamespace { get; set; }

    [XmlAttribute]
    public string Condition { get; set; }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}