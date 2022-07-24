using System.Diagnostics.CodeAnalysis;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace YadaYada.Lambda.Services.Messaging;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public sealed class TenantQueueMessageService<TQueueReaderClass, TTargetClass, TMessage> : ITenantQueueMessageService<TQueueReaderClass,TTargetClass, TMessage> where TTargetClass : class where TQueueReaderClass : class where TMessage : TenantBasedMessage
{
    private readonly IQueueUriProvider<TQueueReaderClass> _queueUriProvider;
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger _logger;
    private Guid _tenantId;

    public TenantQueueMessageService([NotNull] IEnumerable<Claim> claims, IQueueUriProvider<TQueueReaderClass> queueUriProvider, IAmazonSQS sqsClient, ILoggerProvider loggerProvider)
    {
        this._logger = loggerProvider.CreateLogger(this.GetType().FullName!);
        if (!claims.TryGetTenantId(out _tenantId)) throw new InvalidOperationException($"Cannot get {nameof(_tenantId)} from {nameof(claims)}");
        this._queueUriProvider = queueUriProvider;
        this._sqsClient = sqsClient;
    }

    public async Task<SendMessageResponse> SendMessageAsync(TMessage message)
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
                    message.TenantId = _tenantId;
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
                _logger.LogError(e,e.Message);
                throw;
            }
        }
    }
}