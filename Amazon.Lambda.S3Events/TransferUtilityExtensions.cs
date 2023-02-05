using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

// ReSharper disable once CheckNamespace
namespace Amazon.S3.Transfer;

public static class TransferUtilityExtensions
{
    public class MetadataKeys
    {
        public const string TenantId = "tenantid";
    }

    public static Task DownloadAsync(this ITransferUtility transfer, FileInfo fileInfo, string s3Coordinates)
    {
        string bucket;
        string key;
        if (s3Coordinates.StartsWith("s3://"))
        {
            var splits = s3Coordinates.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            bucket = splits[1];
            key = string.Join('/', splits.Skip(2));
        }
        else if (s3Coordinates.StartsWith("arn"))
        {
            var arnSplits = s3Coordinates.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var splits = arnSplits.Last().Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            bucket = splits[0];
            key = string.Join('/', splits.Skip(1));
        }
        else
        {
            throw new NotSupportedException($"The {nameof(s3Coordinates)} of '{s3Coordinates}' is not supported.");
        }
        return transfer.DownloadAsync(fileInfo.FullName, bucket, key);
    }

    public static async Task<S3Object> UploadAsync(this ITransferUtility transferUtility, FileInfo file, string bucketName, string key, Guid tenantId)
    {
        if (transferUtility == null) throw new ArgumentNullException(nameof(transferUtility));
        if (file == null) throw new ArgumentNullException(nameof(file));
        if (bucketName == null) throw new ArgumentNullException(nameof(bucketName));
        if (key == null) throw new ArgumentNullException(nameof(key));
        if (tenantId == Guid.Empty) throw new ArgumentNullException(nameof(tenantId));

        if (!key.TrimStart('/').StartsWith(tenantId.ToString(), StringComparison.InvariantCultureIgnoreCase))
        {
            key = $"/{tenantId}/".Replace("//","/");
        }

        var transferUtilityUploadRequest = new TransferUtilityUploadRequest
        {
            Key = key,
            BucketName = bucketName,
            FilePath = file.FullName
        };

        transferUtilityUploadRequest.Metadata.Add(MetadataKeys.TenantId,tenantId.ToString());

        await transferUtility.UploadAsync(transferUtilityUploadRequest);

        return new S3Object {BucketName = bucketName, Key = key};

    }
}