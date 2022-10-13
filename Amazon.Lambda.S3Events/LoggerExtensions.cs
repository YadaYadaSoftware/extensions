using Amazon.Runtime.Internal.Util;
using Amazon.S3.Model;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Logging;

public static class LoggerExtensions
{
    public static IDisposable AddScope(this ILogger logger, S3Object s3Object)
    {
        AggregateScope aggregateScope = new()
        {
            Capacity = 2
        };
        aggregateScope.Add(logger.AddScope($"{nameof(S3Object)}.{nameof(s3Object.BucketName)}", s3Object.BucketName));
        aggregateScope.Add(logger.AddScope($"{nameof(S3Object)}.{nameof(s3Object.Key)}", s3Object.Key));

        return aggregateScope;

    }
    public static IDisposable AddScope(this ILogger logger, GetObjectResponse s3Object)
    {
        AggregateScope aggregateScope = new()
        {
            Capacity = 2
        };
        aggregateScope.Add(logger.AddScope($"{nameof(S3Object)}.{nameof(s3Object.BucketName)}", s3Object.BucketName));
        aggregateScope.Add(logger.AddScope($"{nameof(S3Object)}.{nameof(s3Object.Key)}", s3Object.Key));

        return aggregateScope;

    }

}