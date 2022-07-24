using Amazon.Lambda.Core;
using Amazon.Lambda.Core.Efs;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class EfsMountTemporaryDirectoryFactoryExtensions
{
    public static IServiceCollection AddEfsMountTemporaryDirectory(this IServiceCollection collection)
    {
        collection.TryAddSingleton<ITemporaryDirectoryFactory, EfsMountTemporaryDirectoryFactory>();
        return collection;
    }

}