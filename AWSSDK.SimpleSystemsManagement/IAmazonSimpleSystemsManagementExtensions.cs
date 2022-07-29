using System.Diagnostics;
using System.Text.Json;
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
        public static async Task<bool> ParameterExistsAsync(this IAmazonSimpleSystemsManagement client, string parameterPath, CancellationToken cancellationToken = default(CancellationToken))
        {

            try
            {
                if (client == null) throw new ArgumentNullException(nameof(client));
                if (parameterPath == null) throw new ArgumentNullException(nameof(parameterPath));
                Console.WriteLine($"{nameof(ParameterExistsAsync)}{nameof(parameterPath)}='{parameterPath}'");


                var describeParametersRequest = new DescribeParametersRequest {
                    ParameterFilters = new List<ParameterStringFilter>
                    {
                        new ParameterStringFilter
                        {
                            Key = "Path",
                            Values = new List<string>
                            {
                                parameterPath
                            }
                        }
                    }
                };

                Console.WriteLine($"{nameof(ParameterExistsAsync)}{nameof(describeParametersRequest)}='{JsonSerializer.Serialize(describeParametersRequest)}'");

                DescribeParametersResponse? describeParametersResponse = await client.DescribeParametersAsync(describeParametersRequest, cancellationToken);
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
