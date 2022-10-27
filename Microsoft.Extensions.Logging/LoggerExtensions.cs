using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Logging;
public static class LoggerExtensions
{
    public static IDisposable AddMember(this ILogger logger, [CallerMemberName] string member = null)
    {
        return logger.AddScope((string)nameof(System.Reflection.MemberInfo), (object)member);
    }

    public static IDisposable AddScope(this ILogger logger, string key, object value)
    {
        return logger.BeginScope(new KeyValuePair<string, object>(key, value));
    }
    public static IDisposable AddScope(this ILogger logger, object value, [CallerArgumentExpression("value")] string? key = null)
    {
        if (key == null) throw new ArgumentNullException(nameof(key));
        return logger.BeginScope(new KeyValuePair<string, object>(key, value));
    }
}

