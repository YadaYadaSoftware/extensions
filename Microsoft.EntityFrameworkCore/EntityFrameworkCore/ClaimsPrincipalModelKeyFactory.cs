using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Claims;

namespace YadaYada.Data.Library;


public class ClaimsPrincipalModelKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime)
    {
        
        var claims = ((ContextBase) context).Claims;
        string returnValue = string.Empty;

        var enumerable = claims as Claim[] ?? claims.ToArray();
        if (enumerable.TryGetTenantId(out Guid tenantId))
        {
            returnValue += tenantId.ToString();
        }
        if (enumerable.TryGetAmazonSellerId(out string sellerId))
        {
            returnValue += sellerId;
        }
        if (enumerable.TryGetNameIdentifier(out string nameIdentifier))
        {
            returnValue += nameIdentifier;
        }

        if (string.IsNullOrEmpty(returnValue))
        {
            throw new InvalidOperationException($"Unable to determine {nameof(ClaimsPrincipalModelKeyFactory)}");
        }

        return (returnValue, designTime);
    }
}