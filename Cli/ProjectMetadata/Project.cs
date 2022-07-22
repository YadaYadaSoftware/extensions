using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;

namespace YadaYada.BuildTools.Cli.ProjectMetadata;

[XmlRoot(elementName: "Project")]
[DebuggerDisplay("{PackageId}")]
public class Project
{
    [XmlIgnore]
    public string PackageId
    {
        get
        {
            var returnValue = string.Empty;
            if (this.File != null && this.File.FullName.Contains("Test") && !this.IsPackable) return returnValue;
            if (string.IsNullOrEmpty(returnValue))
            {
                returnValue = this.PropertyGroup.SingleOrDefault(_ => !string.IsNullOrEmpty(_.PackageId))?.PackageId;
            }

            if (string.IsNullOrEmpty(returnValue))
            {
                returnValue = this.PropertyGroup.SingleOrDefault(_ => !string.IsNullOrEmpty(_.AssemblyName))?.AssemblyName;

            }

            if (string.IsNullOrEmpty(returnValue))
            {
                returnValue = this.PropertyGroup.SingleOrDefault(_ => !string.IsNullOrEmpty(_.RootNamespace))?.RootNamespace;
            }
            return returnValue;
        }
    }

    [XmlElement(elementName: "ItemGroup")]
    public List<ItemGroup> ItemGroup { get; set; } = new List<ItemGroup>();
    [XmlElement(elementName: nameof(PropertyGroup))]
    public List<PropertyGroup> PropertyGroup { get; set; } = new List<PropertyGroup>();

    [XmlIgnore]
    public List<PackageReference> PackageReferences => this.ItemGroup.SelectMany(_ => _.PackageReferences).Select(_ => _).ToList();

    public List<PackageReference> GetPackageReferences(string condition) => 
        this.ItemGroup.Where(_=> (string.IsNullOrEmpty(condition)&&string.IsNullOrEmpty(_.Condition)) || (!string.IsNullOrEmpty(condition) && condition.Equals(_.Condition, StringComparison.InvariantCultureIgnoreCase)))
            .SelectMany(_ => _.PackageReferences).Select(_ => _).ToList();

    public List<ProjectReference> GetProjectReferences(string condition) => 
        this.ItemGroup.Where(_ => condition.Equals(_.Condition, StringComparison.InvariantCultureIgnoreCase))
            .SelectMany(_ => _.ProjectReferences).Select(_ => _).ToList();

    public List<ItemGroup> GetItemGroups(string configurationMaster)
    {
        return this.ItemGroup.Where(_ =>
            configurationMaster.Equals(_.Condition, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }


    [XmlIgnore] public bool IsPackable => this.PropertyGroup.Any(_ => _.IsPackable);



    public static List<Project> From([NotNull] DirectoryInfo directoryInfo)
    {
        var returnValue = new List<Project>();
        foreach (var fileInfo in directoryInfo.GetFiles("*.csproj", SearchOption.AllDirectories))
        {
            var project = From(fileInfo);
            returnValue.Add(project);
        }

        return returnValue;
    }


    public static Project From([NotNull] FileInfo fileInfo)
    {
        if (fileInfo is null || !fileInfo.Exists) throw new ArgumentNullException(nameof(fileInfo));
        List<XmlSerializer> deSerializers = new List<XmlSerializer>();
        deSerializers.Add(new XmlSerializer(typeof(Project)));
        deSerializers.Add(new XmlSerializer(typeof(Project), defaultNamespace: "http://schemas.microsoft.com/developer/msbuild/2003"));
        using var stream = System.IO.File.OpenRead(fileInfo.FullName);
        using var reader = new XmlTextReader(stream);
        Project p = null;
        foreach (var s in deSerializers)
        {
            try
            {

                p = s.Deserialize(reader) as Project;
                break;

            }
            catch (Exception)
            {
                // keep trying
            }

        }

        p.File = fileInfo;

        return p;

        //foreach (var itemGroup in p.ItemGroup)
        //{

        //    if (itemGroup.PackageReferences?.Length > 0)
        //    {
        //        foreach (var itemGroupPackageReference in itemGroup.PackageReferences)
        //        {
        //            returnValue.Metadata.Dependencies.Group.Dependency.Add(new Dependency { Id = itemGroupPackageReference.Include });
        //        }
        //    }
        //}
        //foreach (var propertyGroup in p.PropertyGroup)
        //{
        //    if (!string.IsNullOrEmpty(propertyGroup.PackageId))
        //    {
        //        returnValue.Metadata.Id = propertyGroup.PackageId;
        //    }
        //}

        //if (string.IsNullOrEmpty(returnValue.Metadata.Id))
        //{
        //    foreach (var propertyGroup in p.PropertyGroup)
        //    {
        //        if (!string.IsNullOrEmpty(propertyGroup.AssemblyName))
        //        {
        //            returnValue.Metadata.Id = propertyGroup.AssemblyName;
        //        }
        //    }

        //}

        //if (string.IsNullOrEmpty(returnValue.Metadata.Id))
        //{
        //    foreach (var propertyGroup in p.PropertyGroup)
        //    {
        //        if (!string.IsNullOrEmpty(propertyGroup.RootNamespace))
        //        {
        //            returnValue.Metadata.Id = propertyGroup.RootNamespace;
        //        }
        //    }

        //}
        //if (string.IsNullOrEmpty(returnValue.Metadata.Id))
        //{
        //    returnValue.Metadata.Id = $"{repoUrl}/{file.Name}";
        //}

        //if (string.IsNullOrEmpty(returnValue.Metadata.Repository?.Url))
        //{
        //    returnValue.Metadata.Repository.Url = repoUrl.ToString();
        //}
    }
    [XmlIgnore]
    public FileInfo File { get; set; }

}