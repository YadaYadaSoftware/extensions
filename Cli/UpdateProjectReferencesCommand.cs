using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using YadaYada.BuildTools.Cli.ProjectMetadata;

namespace YadaYada.BuildTools.Cli
{
    public class UpdateProjectReferencesCommand
    {
        [Option('p', "project", Required = true, HelpText = "Project Path")]
        public string ProjectPath { get; set; }

        public static void Process(string projectPath)
        {
            var p = Project.From(new FileInfo(projectPath));
            p.ItemGroup.ForEach(_=>Console.WriteLine(_.ToString()));
        }
    }
}
