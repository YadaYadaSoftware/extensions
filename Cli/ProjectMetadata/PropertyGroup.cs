using System.Xml.Serialization;

namespace YadaYada.BuildTools.Cli.ProjectMetadata;

[XmlRoot(elementName: nameof(PropertyGroup))]
public class PropertyGroup
{
    [XmlElement(elementName: nameof(PackageId))]
    public string PackageId { get; set; }

    [XmlElement(elementName: nameof(AssemblyName))]
    public string AssemblyName { get; set; }

    [XmlElement(elementName: nameof(RootNamespace))]
    public string RootNamespace { get; set; }

    [XmlElement(elementName: nameof(IsPackable))]
    public bool IsPackable { get; set; }
}