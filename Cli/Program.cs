// See https://aka.ms/new-console-template for more information

using Amazon.CloudFormation;
using CommandLine;
using YadaYada.BuildTools.Cli;

Console.WriteLine("Hello, World!");

Parser.Default.ParseArguments<UpdateProjectReferencesCommand>(args)
    .WithParsed<UpdateProjectReferencesCommand>(o =>
    {
    });

await (await Parser.Default.ParseArguments<UpdateProjectReferencesCommand, UpdateTemplateUrlCommand>(args)
        .WithParsedAsync<UpdateProjectReferencesCommand>(async o =>
        {
        }))
    .WithParsedAsync<UpdateTemplateUrlCommand>(async o =>
    {
        await o.ApplyAsync();
    });

public abstract class CommandBase
{
    public abstract Task ApplyAsync();
}

[Verb("update-template-url", HelpText = "Updates a TemplateUrl property in a template")]
public class UpdateTemplateUrlCommand : CommandBase
{
    [Option('t',"template")]
    public FileInfo Template { get; set; }

    [Option('r', "resource")]
    public string Resource { get; set; }

    [Option('u', "template-url")]
    public Uri TemplateUrl { get; set; }
    public override Task ApplyAsync()
    {
        Console.WriteLine($"{nameof(ApplyAsync)}:{nameof(Template)}={Template}, {nameof(Resource)}={Resource}, {nameof(TemplateUrl)}={TemplateUrl}");
        TemplateUpdater.UpdateTemplateAsync(this.Template, this.Resource, "TemplateURL", this.TemplateUrl.ToString());
        return Task.CompletedTask;
    }
}