using Amazon.Lambda.SQSEvents;

namespace Microsoft.Extensions.Logging;

public static class LoggerExtensions
{
    public static IDisposable AddScope(this ILogger logger, SQSEvent.SQSMessage message)
    {
        AggregateScope aggregateScope = new()
        {
            Capacity = message.MessageAttributes.Count + 1
        };
        aggregateScope.Add(logger.AddScope(nameof(message.MessageId), message.MessageId));

        foreach (var (key, value) in message.MessageAttributes)
        {
            aggregateScope.Add(logger.AddScope(key, value.StringValue));
        }

        return aggregateScope;
    }

}