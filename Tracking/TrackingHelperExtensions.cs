using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FedEx.Tracking;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static  class TrackingHelperExtensions
    {
        public static IServiceCollection AddFedExTrackingHelper(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            if (configuration.GetSection("FedEx") == null) throw new InvalidOperationException("FedEx configuration missing.");
            serviceCollection.AddOptions<FedExOptions>("FedEx");
            return serviceCollection.AddSingleton<TrackingHelper>();
        }
    }
}
