using Amazon.Lambda.S3Events;
using Amazon.S3.Transfer;
using Moq;

namespace YadaYada.Lambda;

public class TransferUtilityExtensionsTest
{
    [Fact]
    public void DownloadAsyncTest()
    {
        var transferUtilityMock = new Mock<ITransferUtility>();
        const string fileNameJustName = "blah.zip";
        const string fileName = $"/root/{fileNameJustName}";
        var fileInfo = new FileInfo(fileName);
        const string bucket = "yadayada-master-deploy-codepipelinebucket-18vmytr5wzbha";
        const string key = "githubspike/2022.208.148/x/efbundle-linux-x64";
        transferUtilityMock.Setup(_=>_.DownloadAsync(It.Is<string>(name=>name.EndsWith(fileNameJustName)), bucket, key, CancellationToken.None )).Verifiable();
        transferUtilityMock.Object.DownloadAsync(fileInfo, $"s3://{bucket}/{key}");
        transferUtilityMock.VerifyAll();

    }
}