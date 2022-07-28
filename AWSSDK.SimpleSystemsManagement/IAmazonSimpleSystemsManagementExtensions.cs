using Amazon.SimpleSystemsManagement.Model;

namespace Amazon.SimpleSystemsManagement
{
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public static class IAmazonSimpleSystemsManagementExtensions
    {
        // ReSharper disable once UnusedMember.Global
        public static async Task<string> GetParameterValueAsync(this IAmazonSimpleSystemsManagement client, string parameterPath)
        {
            var value = await client.GetParameterAsync(new GetParameterRequest {Name = parameterPath, WithDecryption = true});
            return value.Parameter.Value;
        }

        // ReSharper disable once UnusedMember.Global
        public static async Task<bool> ParameterExistsAsync(this IAmazonSimpleSystemsManagement client, string parameterPath)
        {
            var results = await client.DescribeParametersAsync(new DescribeParametersRequest {ParameterFilters = new List<ParameterStringFilter> {new ParameterStringFilter {Key = "Name", Values = new List<string> {parameterPath}}}});
            return results.Parameters.Any();
        }

    }
}
