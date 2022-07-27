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
            return transfer.DownloadAsync("/root/blah.zip","yadayada-master-deploy-codepipelinebucket-18vmytr5wzbha","githubspike/2022.208.148/x/efbundle-linux-x64");
        }
    }
}
