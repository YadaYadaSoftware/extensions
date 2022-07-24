using System;

namespace YadaYada.Lambda.Services.Messaging;

public class TenantBasedMessage<TTarget> : TenantBasedMessage
{
    public TenantBasedMessage(Guid eventId, Uri queueUri, string body) : base(eventId, body)
    {
        this.MessageType = typeof(TTarget);
    }
}