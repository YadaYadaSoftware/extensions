using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Amazon.CloudFormation.Model;
using Amazon.Runtime.Internal.Util;

namespace Amazon.CloudFormation
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public static class IAmazonCloudFormationExtensions
    {
        public static async Task<string> GetTemplateBody(this IAmazonCloudFormation cloudFormation, string stackName)
        {
            var templateResponse = await cloudFormation.GetTemplateAsync(new GetTemplateRequest() { StackName = stackName, TemplateStage = TemplateStage.Original });
            return templateResponse.TemplateBody;
        }

        // ReSharper disable once UnusedMember.Global
        public static async Task<Uri> GetTemplateUrl(this IAmazonCloudFormation amazonCloudFormation, string stackName, string resourceName)
        {
            var templateBody = await amazonCloudFormation.GetTemplateBody(stackName);
            var templateNode = JsonNode.Parse(templateBody);
            ArgumentNullException.ThrowIfNull(templateNode, nameof(templateNode));
            var resources = templateNode["Resources"];
            ArgumentNullException.ThrowIfNull(resources, nameof(resources));
            var resource = resources[resourceName];
            ArgumentNullException.ThrowIfNull(resource, nameof(resource));
            var properties = resource["Properties"];
            ArgumentNullException.ThrowIfNull(properties, nameof(properties));
            var templateUrl = properties["TemplateURL"];
            ArgumentNullException.ThrowIfNull(templateUrl, nameof(templateUrl));
            return new Uri(templateUrl.ToString());

        }
    }
}
