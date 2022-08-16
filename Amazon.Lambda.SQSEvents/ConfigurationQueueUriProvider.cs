using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YadaYada.Lambda;
using YadaYada.Lambda.Services;

namespace Amazon.Lambda.SQSEvents;

public class ConfigurationQueueUriProvider<TTargetClass> : IQueueUriProvider<TTargetClass> where TTargetClass : class
{
    private readonly QueueConfiguration _options;
    private readonly ILogger _logger;


    public ConfigurationQueueUriProvider(IOptions<QueueConfiguration> options, ILoggerProvider loggerProvider)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (!options.Value.Any()) throw new ArgumentNullException(nameof(options), "There are no queue configurations in the options.");
        _options = options.Value;
        this._logger = loggerProvider.CreateLogger(this.GetType().FullName!);
    }
    public async Task<Uri> GetQueueUriAsync(Expression<Action<TTargetClass>> expression)
    {
        using (this._logger.AddMember(nameof(GetQueueUriAsync)))
        {
            try
            {
                if (expression is null) throw new ArgumentNullException(nameof(expression));

                if (expression?.Body is MethodCallExpression member)
                {
                    this._logger.LogTrace(expression?.Body?.ToString());
                    var methodInfo = typeof(TTargetClass).GetMethod(member.Method.Name);
                    ArgumentNullException.ThrowIfNull(methodInfo,nameof(methodInfo));
                    return await this.GetQueueUriAsync(methodInfo);
                }

                throw new NotSupportedException();

            }
            catch (Exception e)
            {
                this._logger.LogError(e,e.Message);
                throw;
            }
        }
    }
    public async Task<Uri> GetQueueUriAsync([NotNull] MethodInfo methodInfo)
    {
        using (this._logger.AddMember(nameof(GetQueueUriAsync)))
        using (this._logger.AddScope(nameof(TTargetClass), typeof(TTargetClass).FullName!))
        using (this._logger.AddScope(nameof(methodInfo), methodInfo.Name))
        {
            try
            {
                if (methodInfo is null) throw new ArgumentNullException(nameof(methodInfo));

                var queueKey = $"{typeof(TTargetClass).FullName}.{methodInfo.Name}";
                using (_logger.AddScope(nameof(queueKey), queueKey))
                {
                    try
                    {
                        return new Uri(_options[queueKey]);

                    }
                    catch (Exception e)
                    {

                        this._logger.LogError(e, $"{System.Text.Json.JsonSerializer.Serialize(_options)}{Environment.NewLine}{e.Message}");
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e,$"{System.Text.Json.JsonSerializer.Serialize(_options)}{Environment.NewLine}{e.Message}");
                throw;
            }                

        }
    }

    public async Task<Uri> GetQueueUriAsync()
    {
        using (this._logger.AddMember(nameof(GetQueueUriAsync)))
        {
            try
            {
                MethodInfo methodInfo = null;
                var methodInfos = typeof(TTargetClass).GetMethods().Where(info => info.GetParameters().FirstOrDefault()?.ParameterType == typeof(SQSEvent));

                switch (methodInfos.Count())
                {
                    case 1:
                        methodInfo = methodInfos.Single();
                        break;
                    case var x when x > 1:
                        var markedWithAttribute = methodInfos.Where(_ => _.GetCustomAttribute<DefaultSqsAttribute>() is { });
                        switch (markedWithAttribute.Count())
                        {
                            case 0:
                                throw new InvalidOperationException($"{typeof(TTargetClass)} does not have 1 {nameof(MethodInfo)} with a 1st parameter of {typeof(SQSEvent)}.  It has {methodInfos.Count()} and none are marked with '{nameof(DefaultSqsAttribute)}'.");
                            case 1:
                                methodInfo = markedWithAttribute.Single();
                                break;
                            case var n when n > 1:
                                throw new InvalidOperationException($"{typeof(TTargetClass)} does not have 1 {nameof(MethodInfo)} with a 1st parameter of {typeof(SQSEvent)}.  It has {methodInfos.Count()} and more than one are marked with '{nameof(DefaultSqsAttribute)}'.");

                        }
                        break;
                    default:
                        throw new InvalidOperationException($"{typeof(TTargetClass)} does not have any {nameof(MethodInfo)} with a 1st parameter of {typeof(SQSEvent)}.");
                }

                return await this.GetQueueUriAsync(methodInfo);

            }
            catch (Exception e)
            {
                this._logger.LogError(e,e.Message);
                throw;
            }
        }
    }
}