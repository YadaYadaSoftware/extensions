using FluentAssertions;

namespace Amazon.CloudFormation.Test
{
    public class TemplateUpdaterTest
    {
        [Fact]
        public void UpdateBlueTemplate()
        {
            TemplateUpdater.UpdateTemplate(new FileInfo("data/master.template"), "Blue", "TemplateURL", "s3://nowhere/not.template");
        }

        [Fact]
        public async void UpdateFileNotFound()
        {
            Action action = () => TemplateUpdater.UpdateTemplate(new FileInfo(Path.GetRandomFileName()), "s", "s", "s");

            action.Should().Throw<FileNotFoundException>();
        }
    }
}