using System.Text.Json.Nodes;
using FluentAssertions;

namespace Amazon.CloudFormation.Test
{
    public class TemplateUpdaterTest
    {
        [Fact]
        public async void UpdateBlueTemplate()
        {
            var templateFile = new FileInfo("data/master.template");
            const string s3NowhereNotTemplate = "s3://nowhere/not.template";
            await TemplateUpdater.UpdateTemplateAsync(templateFile, "Blue", "TemplateURL", s3NowhereNotTemplate);

            var json = await File.ReadAllTextAsync(templateFile.FullName);

            var templateNode = JsonNode.Parse(json);
            ArgumentNullException.ThrowIfNull(templateNode, nameof(templateNode));

            var resources = templateNode["Resources"];
            ArgumentNullException.ThrowIfNull(resources,nameof(resources));

            var blue = resources["Blue"];
            ArgumentNullException.ThrowIfNull(blue,nameof(blue));

            var properties = blue["Properties"];
            ArgumentNullException.ThrowIfNull(properties, nameof(properties));

            var templateUrl = properties["TemplateURL"];
            ArgumentNullException.ThrowIfNull(templateUrl, nameof(templateUrl));

            var value = templateUrl.GetValue<string>();
            ArgumentNullException.ThrowIfNull(value,nameof(value));

            value.Should().Be(s3NowhereNotTemplate);


        }

        [Fact]
        public async void UpdateFileNotFound()
        {
            Action action = () => TemplateUpdater.UpdateTemplateAsync(new FileInfo(Path.GetRandomFileName()), "s", "s", "s");

            action.Should().Throw<FileNotFoundException>();
        }
    }
}