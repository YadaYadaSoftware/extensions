using Amazon.Lambda.Core;

namespace Amazon.Lambda.SQSEvents;

public interface ISqsEventHandler
{
    Task HandleSqsMessageAsync(SQSEvent.SQSMessage message, ILambdaContext lambdaContext);
}