using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

namespace Amazon.Lambda.S3Events
{
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

        public static Task UploadAsync(this ITransferUtility transferUtility, FileInfo file, string bucketName, string key, Guid tenantId)
        {
            if (transferUtility == null) throw new ArgumentNullException(nameof(transferUtility));
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (bucketName == null) throw new ArgumentNullException(nameof(bucketName));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (tenantId == Guid.Empty) throw new ArgumentNullException(nameof(tenantId));

            var transferUtilityUploadRequest = new TransferUtilityUploadRequest
            {
                Key = key,
                BucketName = bucketName,
                FilePath = file.FullName
            };

            transferUtilityUploadRequest.Metadata.Add(MetadataKeys.TenantId,tenantId.ToString());

            return transferUtility.UploadAsync(transferUtilityUploadRequest);

        }
    }
}
