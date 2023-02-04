namespace Amazon.Lambda.SQSEvents;

public class TenantBasedMessage<TTarget> : TenantBasedMessage
{
    public TenantBasedMessage(string eventId, Uri queueUri, string body) : base((string) eventId, body)
    {
        this.MessageType = typeof(TTarget);
    }
}