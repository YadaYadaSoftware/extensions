using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Amazon.CloudFormation.Model;
using Amazon.Runtime.Internal.Util;
using InvalidOperationException = Amazon.CloudFormation.Model.InvalidOperationException;

namespace Amazon.CloudFormation
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public static class IAmazonCloudFormationExtensions
    {
        // ReSharper disable once UnusedMember.Global
        public static async Task<string> GetTemplateAsync(this IAmazonCloudFormation cloudFormation, string masterStack, string nestedStackLogicalId)
        {
            var nestedStack = await cloudFormation.DescribeStackResourceAsync(new DescribeStackResourceRequest {StackName = masterStack, LogicalResourceId = nestedStackLogicalId});

            var template = await cloudFormation.GetTemplateAsync(nestedStack.StackResourceDetail.PhysicalResourceId);

            return template;
        }
        public static async Task<string> GetTemplateAsync(this IAmazonCloudFormation cloudFormation, string stackName)
        {
            var templateResponse = await cloudFormation.GetTemplateAsync(new GetTemplateRequest() { StackName = stackName, TemplateStage = TemplateStage.Original });
            return templateResponse.TemplateBody;
        }

        // ReSharper disable once UnusedMember.Global
        public static async Task<Uri> GetTemplateUrlAsync(this IAmazonCloudFormation amazonCloudFormation, string stackName, string resourceName)
        {
            var templateBody = await amazonCloudFormation.GetTemplateAsync(stackName);
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

        // ReSharper disable once UnusedMember.Global
        public static async Task<bool> StackExists(this IAmazonCloudFormation amazonCloudFormation, string stackName)
        {
            DescribeStacksResponse describeStacksAsync = null;
            do
            {
                DescribeStacksRequest describeStacksRequest = new DescribeStacksRequest { NextToken = describeStacksAsync?.NextToken };
                describeStacksAsync = await amazonCloudFormation.DescribeStacksAsync(describeStacksRequest);
                if (describeStacksAsync.Stacks.SingleOrDefault(_ => _.StackName == stackName) is { }) return true;
            } while (!string.IsNullOrEmpty(describeStacksAsync.NextToken));

            return false;
        }


        // ReSharper disable once UnusedMember.Global
        public static async Task<string> GetPropertyValue(this IAmazonCloudFormation amazonCloudFormation, string stackName, string resourceName, string propertyPath)
        {
            var templateBody = await amazonCloudFormation.GetTemplateAsync(stackName);
            var templateNode = JsonNode.Parse(templateBody);
            ArgumentNullException.ThrowIfNull(templateNode, nameof(templateNode));
            var resources = templateNode["Resources"];
            ArgumentNullException.ThrowIfNull(resources, nameof(resources));
            var resource = resources[resourceName];
            ArgumentNullException.ThrowIfNull(resource, nameof(resource));

            var propertyPaths = propertyPath.Split('.', '/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var currentElement = resource["Properties"];

            foreach (var path in propertyPaths)
            {
                ArgumentNullException.ThrowIfNull(currentElement, nameof(currentElement));
                currentElement = currentElement[path];

            }

            ArgumentNullException.ThrowIfNull(currentElement, nameof(currentElement));
            var value = currentElement.GetValue<string>();

            if (string.IsNullOrEmpty(value)) throw new System.InvalidOperationException($"{nameof(value)} is null.");

            return value;



        }



    }
}
