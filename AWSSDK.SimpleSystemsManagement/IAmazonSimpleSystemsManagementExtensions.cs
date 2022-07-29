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
            try
            {
                Console.WriteLine(nameof(ParameterExistsAsync));
                var describeParametersResponse = await client.DescribeParametersAsync(new()
                {
                    ParameterFilters = new() {new()
            {
                Key = "Name", Values = new()
                {
                    parameterPath
                }
            }}
                });
                Console.WriteLine($"{nameof(ParameterExistsAsync)}:{nameof(describeParametersResponse)}={describeParametersResponse}");


                var parameterExistsAsync = describeParametersResponse.Parameters.Any();
                Console.WriteLine($"{parameterPath}.Exists={parameterExistsAsync}");
                return parameterExistsAsync;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

    }
}
