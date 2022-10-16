using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amazon.Lambda.SQSEvents;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class TenantQueueMessageService<TQueueReaderClass, TTargetClass, TMessage> : ITenantQueueMessageService<TQueueReaderClass,TTargetClass, TMessage> where TTargetClass : class where TQueueReaderClass : class where TMessage : TenantBasedMessage
{
    private readonly IQueueUriProvider<TQueueReaderClass> _queueUriProvider;
    private readonly IAmazonSQS _sqsClient;
    private readonly IEnumerable<Claim> _claims;
    private readonly ILogger _logger;
    private Guid _tenantId;

    public TenantQueueMessageService(IQueueUriProvider<TQueueReaderClass> queueUriProvider, IAmazonSQS sqsClient, ILoggerProvider loggerProvider, IEnumerable<Claim> claims)
    {
        this._logger = loggerProvider.CreateLogger(this.GetType().Name!);
        this._queueUriProvider = queueUriProvider;
        this._sqsClient = sqsClient;
        _claims = claims;
    }

    public Task<SendMessageResponse> SendMessageAsync(TMessage message)
    {
        if (_tenantId == default)
        {
            ArgumentNullException.ThrowIfNull(_claims, nameof(_claims));
            if (!_claims.TryGetTenantId(out _tenantId)) throw new InvalidOperationException($"Cannot get {nameof(_tenantId)} from {nameof(_claims)}");
        }
        return this.SendMessageAsync(_tenantId, message);
    }

    public async Task<SendMessageResponse> SendMessageAsync(Guid tenantId, TMessage message)
    {
        using (_logger.AddMember(nameof(SendMessageAsync)))
        {
            try
            {
                var uri = await this._queueUriProvider.GetQueueUriAsync();

                using (_logger.AddScope(nameof(uri), uri))
                using (_logger.AddScope(nameof(message.TenantId), message.TenantId))
                using (_logger.AddScope(nameof(message.EventId), message.EventId))
                {
                    message.QueueUrl = uri.ToString();
                    message.TenantId = tenantId;
                    message.MessageType = typeof(TTargetClass);
                    if (string.IsNullOrEmpty(message.MessageDeduplicationId))
                    {
                        message.MessageDeduplicationId = Guid.NewGuid().ToString();
                    }
                    var sendMessageResponse = await this._sqsClient.SendMessageAsync(message, CancellationToken.None);
                    _logger.LogTrace("{MessageId}=", sendMessageResponse.MessageId);
                    return sendMessageResponse;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}