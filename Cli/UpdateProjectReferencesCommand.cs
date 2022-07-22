using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace YadaYada.BuildTools.Cli
{
    public class UpdateProjectReferencesCommand
    {
        [Option('p', "project", Required = true, HelpText = "Project Path")]
        public string ProjectPath { get; set; }
    }
}
