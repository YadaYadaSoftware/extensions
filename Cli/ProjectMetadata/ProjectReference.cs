using System.Xml.Serialization;

namespace YadaYada.BuildTools.Cli.ProjectMetadata;

[XmlRoot(elementName: "ProjectReference")]
public class ProjectReference
{
    [XmlAttribute(attributeName: "Include")]
    public string Include { get; set; }

}