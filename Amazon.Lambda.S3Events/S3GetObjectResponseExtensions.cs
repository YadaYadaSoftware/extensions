using Amazon.S3.Model;

namespace Amazon.Lambda.S3Events;

public static class S3GetObjectResponseExtensions
{
    public static class Constants
    {
        public static class MetadataKeys
        {
            //x-amz-meta-user-agent-id
            public const string UserAgent = "user-agent-id";
        }
    }

    public static string GetExtension(this GetObjectResponse getObjectResponse) => Path.GetExtension(getObjectResponse.Key.ToLowerInvariant());

    public static bool TryGetUserAgent(this GetObjectResponse getObjectResponse, out string userAgent)
    {
        userAgent = getObjectResponse.Metadata[Constants.MetadataKeys.UserAgent]?.Split('@').First();
        return !string.IsNullOrEmpty(userAgent);
    }
    public static bool TryGetMetadata(this GetObjectResponse getObjectResponse, string key, out Guid result)
    {
        result = default;
        return Guid.TryParse(getObjectResponse.Metadata[key], out result);
    }

    public static bool TryGetMetadata(this GetObjectResponse getObjectResponse, string key, out string result)
    {
        result = getObjectResponse.Metadata[key];
        return !string.IsNullOrEmpty(result);
    }
    public static bool TryGetTenantId(this GetObjectResponse getObjectResponse, out Guid tenantId)
    {
        return TryGetMetadata(getObjectResponse, TransferUtilityExtensions.MetadataKeys.TenantId, out tenantId);
    }
}