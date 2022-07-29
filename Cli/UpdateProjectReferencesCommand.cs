using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using YadaYada.BuildTools.Cli.ProjectMetadata;

namespace YadaYada.BuildTools.Cli
{
    [Verb("update-project-reference")]
    public class UpdateProjectReferencesCommand : CommandBase
    {
        [Option('p', "project-path", Required = true, HelpText = "Project Path")]
        public string ProjectPath { get; set; }
        [Option('o', "output", Required = false, HelpText = "Output")]
        public string Output { get; set; }



        public static void Process(string projectPath, string output)
        {
            var p = Project.From(new FileInfo(projectPath));
            p.ItemGroup.ForEach(_=>Console.WriteLine(_.ToString()));
            var references = p.GetProjectReferences(string.Empty);
            Console.WriteLine(references.Count);
            references.ForEach(_=>Console.WriteLine($"ProjectReference:{_.Include}"));
            foreach (var projectReference in references)
            {
                var fileInfo = new FileInfo(Path.Combine(projectPath, projectReference.Include));
                Console.WriteLine($"{nameof(fileInfo)}:{nameof(fileInfo.Exists)}={fileInfo.Exists}:{fileInfo.FullName}");
            }
            var itemGroup = new ItemGroup();
            itemGroup.PackageReferences.Add(new PackageReference(){Include = "NewtonSoft.Json.Net"});
            p.ItemGroup.Add(itemGroup);
            var outputFileInfo = new FileInfo(output);
            Project.Save(p,outputFileInfo);
            Console.WriteLine(File.ReadAllText(outputFileInfo.FullName));
            
        }

        public override Task ApplyAsync()
        {
            Console.WriteLine($"Update {this.ProjectPath}");
            this.Output = this.Output ?? this.ProjectPath;
            UpdateProjectReferencesCommand.Process(this.ProjectPath, this.Output);
            return Task.CompletedTask;
        }
    }
}
