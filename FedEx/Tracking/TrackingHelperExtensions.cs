using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FedEx.Authorization;
using FedEx.Tracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static  class TrackingHelperExtensions
    {
        public static IServiceCollection AddFedExTrackingHelper(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("FedEx");
            if (configurationSection == null) throw new InvalidOperationException("FedEx configuration missing.");
            serviceCollection.Configure<FedExOptions>(configurationSection);
            serviceCollection.AddSingleton<Token>();
            serviceCollection.TryAddSingleton<TrackingHelper>();
            return serviceCollection;

        }
    }
}
