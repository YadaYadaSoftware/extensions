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
            s3.Setup(x => x.ListObjectsV2Async(It.IsAny<ListObjectsV2Request>(), cancellationToken)).Verifiable();

            await s3.Object.CopyFolderAsync(sourceBucket, string.Empty, string.Empty, string.Empty, cancellationToken);

            s3.VerifyAll();

        }
    }
}
