using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;


public static class TenantMessagingExtensions
{
    public static IServiceCollection AddTenantMessaging<TQueueReader, TTargetType, TMessageType>(
        this IServiceCollection collection, ILogger logger = default)
        where TQueueReader : class where TTargetType : class where TMessageType : TenantBasedMessage
    {
        logger?.LogTrace(nameof(AddTenantMessaging));
        collection.TryAddScoped<ITenantQueueMessageService<TQueueReader, TTargetType, TMessageType>, TenantQueueMessageService<TQueueReader, TTargetType, TMessageType>>();

        return collection.AddQueueUriProvider<TQueueReader>(logger);
    }
}