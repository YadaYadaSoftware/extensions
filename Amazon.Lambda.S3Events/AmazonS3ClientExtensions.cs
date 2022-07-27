using Amazon.S3;
using Amazon.S3.Model;

namespace Amazon.Lambda.S3Events;

public static class AmazonS3ClientExtensions
{
    public static async Task CopyFolderAsync(this IAmazonS3 amazonS3, string sourceBucket,
        string sourceFolder,
        string destinationBucket,
        string destinationFolder,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var listObjectsV2Response = await amazonS3.ListObjectsV2Async(
            new ListObjectsV2Request{Prefix = sourceFolder}, 
            cancellationToken);

        listObjectsV2Response.S3Objects.ForEach(o =>
        {
            var destinationKey = destinationFolder + o.Key.Replace(sourceFolder, string.Empty);
            amazonS3.CopyObjectAsync(o.BucketName, o.Key, destinationBucket, destinationKey, cancellationToken);
        });
    }
}