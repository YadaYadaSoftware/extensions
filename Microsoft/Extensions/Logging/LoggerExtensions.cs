using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Logging;
public static class LoggerExtensions
{
    public static IDisposable AddMember(this ILogger logger, [CallerMemberName] string member = null)
    {
        return logger.AddScope(nameof(System.Reflection.MemberInfo), member);
    }

    public static IDisposable AddScope(this ILogger logger, string key, object value)
    {
        return logger.BeginScope(new KeyValuePair<string, object>(key, value));
    }
}

