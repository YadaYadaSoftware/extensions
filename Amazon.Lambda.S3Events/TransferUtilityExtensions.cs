using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Transfer;

namespace Amazon.Lambda.S3Events
{
    public static class TransferUtilityExtensions
    {
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
                throw new NotSupportedException();
            }
            return transfer.DownloadAsync(fileInfo.FullName, bucket, key);
        }
    }
}
