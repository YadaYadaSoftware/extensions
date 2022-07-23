using Microsoft.Extensions.Logging;

namespace YadaYada.Lambda.Services;

public class LoggerProviderYadaYada : ILoggerProvider, ISupportExternalScope
{
    private readonly LambdaLoggerOptions _lambdaLoggerOptions;
    private IExternalScopeProvider _scopeProvider ;

    public LoggerProviderYadaYada(LambdaLoggerOptions lambdaLoggerOptions)
    {
        _lambdaLoggerOptions = lambdaLoggerOptions;
        _scopeProvider = new LoggerExternalScopeProvider();
    }
    public void Dispose()
    {
        
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new LoggerYadaYada(categoryName, _lambdaLoggerOptions);
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }
}