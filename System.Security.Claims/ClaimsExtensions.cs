using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace System.Security.Claims;



public static class ClaimsExtensions
{
    public enum ClaimTypes
    {
        [EnumMember(Value = ClaimsExtensions.TenantIdClaim)] TenantId,
        [EnumMember(Value = ClaimsExtensions.AmazonSellerIdClaim)] AmazonSellerId,
        [EnumMember(Value = ClaimsExtensions.UserAgentClaim)] UserAgent,
    }

    public const string TenantIdClaim = "https://tenantweb2/tenantId";
    public const string AmazonSellerIdClaim = "https://tenantweb2/amazonSellerId";
    public const string UserAgentClaim = "https://tenantweb2/userAgent";

    public static bool TryGetAmazonSellerId(this IEnumerable<Claim> claims, out string result)
    {
        if (claims?.FirstOrDefault(_ => _.Type == AmazonSellerIdClaim) is { } claim)
        {
            result = claim.Value;
            return true;
        }

        result = null;
        return false;
    }

    public static bool TryGetNameIdentifier(this IEnumerable<Claim> claims, out string result)
    {
        if (claims?.FirstOrDefault(_ => _.Type == System.Security.Claims.ClaimTypes.NameIdentifier) is { } claim)
        {
            result = claim.Value;
            return true;
        }

        result = null;
        return false;
    }
    public static bool TryGetUserAgent(this IEnumerable<Claim> claims, out string userAgent)
    {
        userAgent = default;

        if (claims?.FirstOrDefault(_ => _.Type == UserAgentClaim) is { } claim)
        {
            userAgent = claim.Value;
            return true;
        }

        return false;
    }
    public static bool TryGetTenantId(this IEnumerable<Claim> claims, out Guid result)
    {
        result = default;
        return claims?.FirstOrDefault(_ => _.Type == TenantIdClaim) is { } claim && Guid.TryParse(claim.Value, out result);
    }
    public static string GetAmazonSellerId(this IEnumerable<Claim> claims)
    {
        if (claims?.FirstOrDefault(_ => _.Type == AmazonSellerIdClaim) is { } claim)
            return claim.Value;
        return null;
    }
}

