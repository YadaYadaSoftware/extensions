namespace YadaYada.BuildTools.Cli.ProjectMetadata;

public class ValidateProjectFiles
{
    const string configurationNotMaster = "'$(Configuration)'!='Master'";
    const string configurationMaster = "'$(Configuration)'=='Master'";
    public void Validate(DirectoryInfo rootDirectory)
    {
        var projects = Project.From(rootDirectory);
        int i = 0;
        while (i< projects.Count)
        {
            var project = projects[i];
            if (project.File.FullName.Contains("/labelmaker/")
                || project.File.FullName.Contains("/bbdeploy/")
                || project.File.FullName.Contains("/smellycar/")
               )
            {
                projects.Remove(project);
            }
            else
            {
                i++;
            }
        }
        Parallel.ForEach(projects, project => this.ValidateCountsMatch(project, configurationMaster, configurationNotMaster));
        Parallel.ForEach(projects, this.ValidateYadaPackageReferences);
    }

    public void ValidateYadaPackageReferences(Project project)
    {

        var references = project.GetPackageReferences(null);
        foreach (var packageReference in references)
        {
            if (packageReference.Include.StartsWith("YadaYada"))
            {
                var id = project.PackageId;
                if (string.IsNullOrEmpty(id))
                {
                    id = project.File?.FullName;
                }
                    
                throw new InvalidOperationException($"{project.File?.DirectoryName}:{id} contains an reference to {packageReference.Include} without a {nameof(ItemGroup.Condition)} of '{configurationNotMaster}'.  All {nameof(YadaYada)} Package References must be within an {nameof(ItemGroup)} with a {nameof(ItemGroup.Condition)} of {configurationNotMaster}.");
            }
        }

    }

    public void ValidateProject(Project project)
    {

    }

    public void ValidateCountsMatch(Project project, string configurationMaster, string configurationNotMaster)
    {
        try
        {
            List<ItemGroup> masterItemGroups = project.GetItemGroups(configurationMaster);
            List<ItemGroup> notMasterItemGroups = project.GetItemGroups(configurationNotMaster);
            var masterCount = masterItemGroups.SelectMany(group => @group.ProjectReferences).Count();
            var notMasterCount = notMasterItemGroups.SelectMany(group => @group.PackageReferences).Count();
            if (masterCount != notMasterCount)
            {
                throw new InvalidOperationException($"{project.File?.FullName}: Package reference count does not match Project reference count.");
            }

        }
        catch (Exception e)
        {
            Console.Write($"{project.File?.Name}:{e}");
            throw;
        }
    }
}