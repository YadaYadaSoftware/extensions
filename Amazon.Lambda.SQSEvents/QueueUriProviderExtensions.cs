using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.SQSEvents;

public static class QueueUriProviderExtensions
{
    public static IServiceCollection AddQueueUriProvider<TFunction>(
        this IServiceCollection collection, ILogger logger = null)
        where TFunction : class
    {
        try
        {
            logger?.LogTrace($"{nameof(QueueUriProviderExtensions)}.{nameof(AddQueueUriProvider)}");
            collection.TryAddSingleton<IQueueUriProvider<TFunction>, ConfigurationQueueUriProvider<TFunction>>();
            return collection;

        }
        catch (Exception e)
        {
            logger?.LogError(e,e.Message);
            throw;
        }
    }

}