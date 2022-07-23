using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection TryAddEnumerableScoped<TService, TImplementation>(this IServiceCollection serviceCollection) where TService : class where TImplementation : class, TService
        {
            serviceCollection.TryAddEnumerable(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));
            return serviceCollection;
        }
        public static IServiceCollection TryAddEnumerableSingleton<TService, TImplementation>(this IServiceCollection serviceCollection) where TService : class where TImplementation : class, TService
        {
            serviceCollection.TryAddEnumerable(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));
            return serviceCollection;
        }
    }
}
