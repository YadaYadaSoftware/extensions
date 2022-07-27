using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Moq;

namespace S3Events.Test
{
    public class AmazonS3ClientExtensionsTest
    {
        [Fact]
        public async void CopyFolderAsyncTest()
        {
            var s3 = new Mock<IAmazonS3>();
            const string sourceBucket = "sourceBucket";

            var cancellationToken = new CancellationToken();
            var listObjectsV2Response = new ListObjectsV2Response();
            var fileTxt = "file.txt";
            listObjectsV2Response.S3Objects.Add(new S3Object(){Key = fileTxt, BucketName = sourceBucket});
            s3.Setup(x => x.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken)).ReturnsAsync(listObjectsV2Response).Verifiable();

            s3.Setup(amazonS3 => amazonS3.CopyObjectAsync(sourceBucket, fileTxt, It.IsAny<string>(), It.IsAny<string>(), cancellationToken)).Verifiable();

            await s3.Object.CopyFolderAsync(sourceBucket, string.Empty, string.Empty, string.Empty, cancellationToken);

            s3.VerifyAll();

        }
    }
}
