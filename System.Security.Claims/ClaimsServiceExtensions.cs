using System.Security.Claims;
using YadaYada.Library.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class ClaimsServiceExtensions
{
    public static IServiceCollection AddClaim(this IServiceCollection serviceCollection, ClaimsExtensions.ClaimTypes claimType, string value)
    {
        return serviceCollection.AddClaim(claimType.GetEnumMemberValue(), value);
    }

    private static IServiceCollection AddClaim(this IServiceCollection serviceCollection, string claimType, string value)
    {

        serviceCollection.AddScoped(provider => new Claim(claimType, value));

        return serviceCollection;

    }
}