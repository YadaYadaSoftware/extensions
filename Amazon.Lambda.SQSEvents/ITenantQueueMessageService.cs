using Amazon.SQS.Model;

namespace Amazon.Lambda.SQSEvents;

public interface ITenantQueueMessageService<TQueueReaderClass, TTargetClass, in TMessageType> 
    where TQueueReaderClass : class where TTargetClass : class where TMessageType : TenantBasedMessage
{
    Task<SendMessageResponse> SendMessageAsync(TMessageType message);
}