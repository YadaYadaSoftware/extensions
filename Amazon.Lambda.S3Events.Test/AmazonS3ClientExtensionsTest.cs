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
            const string sourceBucket = "sourceBucket";
            const string fileName = "file.txt";
            const string txtFileFullPath = $"sourcefolder/{fileName}";
            const string sourceFolder = "sourcefolder/";
            const string destinationBucket = "destinationBucket";
            const string destinationFolder = "destinationFolder/";

            var s3 = new Mock<IAmazonS3>();

            var cancellationToken = new CancellationToken();
            var listObjectsV2Response = new ListObjectsV2Response();
            listObjectsV2Response.S3Objects.Add(new S3Object{Key = txtFileFullPath, BucketName = sourceBucket});

            s3.Setup(x => x.ListObjectsV2Async(It.Is<ListObjectsV2Request>(request => request.Prefix == sourceFolder), cancellationToken)).ReturnsAsync(listObjectsV2Response).Verifiable();

            s3.Setup(amazonS3 => amazonS3.CopyObjectAsync(sourceBucket, txtFileFullPath, destinationBucket, destinationFolder + fileName, cancellationToken)).Verifiable();

            await s3.Object.CopyFolderAsync(sourceBucket, sourceFolder, destinationBucket, destinationFolder, cancellationToken);

            s3.VerifyAll();

        }
    }
}
