using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.S3Events;

public static class AmazonS3ClientExtensions
{
    public static Task CopyFolderAsync(this IAmazonS3 amazonS3, string sourceS3Url,
        string destinationBucket,
        string destinationFolder,
        CancellationToken cancellationToken = default(CancellationToken),
        ILogger logger = default)
    {
        if (amazonS3 == null) throw new ArgumentNullException(nameof(amazonS3));
        if (sourceS3Url == null) throw new ArgumentNullException(nameof(sourceS3Url));
        if (destinationBucket == null) throw new ArgumentNullException(nameof(destinationBucket));
        if (destinationFolder == null) throw new ArgumentNullException(nameof(destinationFolder));
        if (string.IsNullOrEmpty(sourceS3Url)) throw new ArgumentException("Value cannot be null or empty.", nameof(sourceS3Url));
        if (string.IsNullOrEmpty(destinationBucket)) throw new ArgumentException("Value cannot be null or empty.", nameof(destinationBucket));
        if (string.IsNullOrEmpty(destinationFolder)) throw new ArgumentException("Value cannot be null or empty.", nameof(destinationFolder));

        AggregateScope? loggerAggregateScope = null;

        if (logger != null)
        {
            loggerAggregateScope = new AggregateScope();
            loggerAggregateScope.Add(logger.AddMember());
            loggerAggregateScope.Add(logger.AddScope(nameof(sourceS3Url), sourceS3Url));
            loggerAggregateScope.Add(logger.AddScope(nameof(destinationBucket), destinationBucket));
            loggerAggregateScope.Add(logger.AddScope(nameof(destinationFolder), destinationFolder));
        }

        try
        {
            //s3://yadayada-master-deploy-codepipelinebucket-18vmytr5wzbha/githubspike/2022.208.156/blazor
            var sourceS3UrlParts = sourceS3Url.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            loggerAggregateScope?.Add(logger?.AddScope(nameof(sourceS3UrlParts),string.Join(',', sourceS3UrlParts)));

            var sourceBucketName = sourceS3UrlParts[1];

            loggerAggregateScope?.Add(logger?.AddScope(nameof(sourceBucketName), sourceBucketName));

            var sourceKey = string.Join('/', sourceS3UrlParts.Skip(2));

            loggerAggregateScope?.Add(logger?.AddScope(nameof(sourceKey), sourceKey));


            return CopyFolderAsync(amazonS3, sourceBucketName, sourceKey, destinationBucket, destinationFolder, cancellationToken);
        }
        catch (Exception e)
        {
            logger?.LogError(e,e.ToString());
            throw;
        }
        finally
        {
            loggerAggregateScope?.Dispose();
        }
    }

    public static async Task CopyFolderAsync(this IAmazonS3 amazonS3, string sourceBucket,
        string sourceFolder,
        string destinationBucket,
        string destinationFolder,
        CancellationToken cancellationToken = default(CancellationToken),
        ILogger? logger = default)
    {

        AggregateScope? loggerAggregateScope = null;

        if (logger != null)
        {
            loggerAggregateScope = new AggregateScope();
            loggerAggregateScope.Add(logger.AddMember());
            loggerAggregateScope.Add(logger.AddScope(nameof(sourceBucket), sourceBucket));
            loggerAggregateScope.Add(logger.AddScope(nameof(sourceFolder), sourceFolder));
            loggerAggregateScope.Add(logger.AddScope(nameof(destinationBucket), destinationBucket));
            loggerAggregateScope.Add(logger.AddScope(nameof(destinationFolder), destinationFolder));

        }

        try
        {
            var listObjectsV2Response = await amazonS3.ListObjectsV2Async(
                new ListObjectsV2Request {BucketName = sourceBucket, Prefix = sourceFolder},
                cancellationToken);

            var tasks = new List<Task>();

            Task[] incompleteTasks;

            listObjectsV2Response.S3Objects.ForEach(async o =>
            {
                var addScopeKey = logger?.AddScope(nameof(o.Key), o.Key);
                loggerAggregateScope?.Add(addScopeKey);
                var destinationKey = destinationFolder + o.Key.Replace(sourceFolder, string.Empty);
                var addScopeDestinationKey = logger?.AddScope(nameof(destinationKey), destinationKey);
                loggerAggregateScope?.Add(addScopeDestinationKey);

                try
                {

                    tasks.Add(amazonS3.CopyObjectAsync(o.BucketName, o.Key, destinationBucket, destinationKey, cancellationToken));

                    incompleteTasks = tasks.Where(_ => !_.IsCompleted).ToArray();
                    while (incompleteTasks.Length > 20)
                    {
                        await Task.WhenAny(incompleteTasks);
                        incompleteTasks = tasks.Where(_ => !_.IsCompleted).ToArray();
                    }

                }
                catch (Exception e)
                {
                    logger?.LogError(e, e.ToString());

                    throw;
                }
                finally
                {
                    loggerAggregateScope?.Remove(addScopeKey);
                    addScopeKey?.Dispose();
                    loggerAggregateScope?.Remove(addScopeDestinationKey);
                    addScopeDestinationKey?.Dispose();
                }
            });

            incompleteTasks = tasks.Where(_ => !_.IsCompleted).ToArray();

            await Task.WhenAll(incompleteTasks);

        }
        catch (Exception e)
        {
            logger?.LogError(e, e.ToString());

            throw;
        }
        finally
        {
            loggerAggregateScope?.Dispose();
        }
    }
}