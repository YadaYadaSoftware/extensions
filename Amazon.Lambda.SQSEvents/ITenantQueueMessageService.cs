using System.Threading.Tasks;
using Amazon.SQS.Model;

namespace YadaYada.Lambda.Services.Messaging;

public interface ITenantQueueMessageService<TQueueReaderClass, TTargetClass, in TMessageType> 
    where TQueueReaderClass : class where TTargetClass : class where TMessageType : TenantBasedMessage
{
    Task<SendMessageResponse> SendMessageAsync(TMessageType message);
}