using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Amazon.Lambda.Core.Settings;

public static class SettingServiceExtensions
{
    public static IServiceCollection AddEnvironmentVariableSettingService(this IServiceCollection collection)
    {
        collection.TryAddSingleton<ISettingService,EnvironmentVariableSettingService>();
        return collection;
    }
}