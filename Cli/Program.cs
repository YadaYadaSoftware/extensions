// See https://aka.ms/new-console-template for more information

using CommandLine;
using YadaYada.BuildTools.Cli;

Console.WriteLine("Hello, World!");

Parser.Default.ParseArguments<UpdateProjectReferencesCommand>(args)
    .WithParsed<UpdateProjectReferencesCommand>(o =>
    {
        Console.WriteLine($"Update {o.ProjectPath}");
    });