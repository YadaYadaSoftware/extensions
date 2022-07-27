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
            const string fileName1 = "file.txt";
            const string fileNamePath1 = $"sourcefolder/{fileName1}";
            const string fileName2 = "file2.txt";
            const string fileNamePath2 = $"sourcefolder/anotherfolder/anotherfolder/{fileName2}";
            const string sourceFolder = "sourcefolder/";
            const string destinationBucket = "destinationBucket";
            const string destinationFolder = "destinationFolder/";

            var s3 = new Mock<IAmazonS3>();

            var cancellationToken = new CancellationToken();
            var listObjectsV2Response = new ListObjectsV2Response();
            listObjectsV2Response.S3Objects.Add(new S3Object { Key = fileNamePath1, BucketName = sourceBucket });
            listObjectsV2Response.S3Objects.Add(new S3Object { Key = fileNamePath2, BucketName = sourceBucket });

            s3.Setup(x => x.ListObjectsV2Async(It.Is<ListObjectsV2Request>(request => request.Prefix == sourceFolder && request.BucketName == sourceBucket), cancellationToken)).ReturnsAsync(listObjectsV2Response).Verifiable();

            s3.Setup(amazonS3 => amazonS3.CopyObjectAsync(sourceBucket, fileNamePath1, destinationBucket, destinationFolder + fileName1, cancellationToken)).Verifiable();
            s3.Setup(amazonS3 => amazonS3.CopyObjectAsync(sourceBucket, fileNamePath2, destinationBucket, destinationFolder + $"anotherfolder/anotherfolder/{fileName2}", cancellationToken)).Verifiable();

            await s3.Object.CopyFolderAsync(sourceBucket, sourceFolder, destinationBucket, destinationFolder, cancellationToken);

            s3.VerifyAll();

        }
    }
}
