using Amazon.S3.Model;

namespace Amazon.Lambda.S3Events;

public static class S3GetObjectResponseExtensions
{
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
}