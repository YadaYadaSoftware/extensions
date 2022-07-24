using Amazon.Lambda.Core;
using Amazon.S3.Model;

namespace Amazon.Lambda.S3Events;

public interface IS3EventHandler
{
    Task HandleNewS3ObjectAsync(Guid eventId, S3Object s3Object, ILambdaContext lambdaContext);
    Task<bool> CanHandleAsync(S3Object s3Object, ILambdaContext lambdaContext);
}