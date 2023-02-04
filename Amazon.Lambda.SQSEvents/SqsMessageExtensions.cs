// ReSharper disable once CheckNamespace
namespace Amazon.Lambda.SQSEvents;

public static class SqsMessageExtensions
{

    //public static class Constants
    //{
    //    public static class MessageAttributes
    //    {
    //        public const string TenantIdKey = "TenantId";
    //        public const string MessageType = "MessageType";
    //        public const string EventId = "EventId";
    //    }
    //}
    public static bool TryGetTenantId(this SQSEvent.SQSMessage message, out Guid tenantId)
    {
        tenantId = default;

        return message.MessageAttributes.SingleOrDefault(_ => _.Key == nameof(TenantBasedMessage.TenantId)) is {Value: { } value}
               && Guid.TryParse(value.StringValue, out tenantId);
    }


    public static bool TryGetHandlerType(this SQSEvent.SQSMessage message, out Type type)
    {
        type = default;
        if (message.MessageAttributes.SingleOrDefault(_ => _.Key == nameof(TenantBasedMessage.MessageType)) is not {Value: { } value}) return false;
        type = Type.GetType(value.StringValue);
        return type != default;
    }
    public static bool TryGetEventId(this SQSEvent.SQSMessage message, out string? eventId)
    {
        if (message.MessageAttributes.SingleOrDefault(_ => _.Key == nameof(TenantBasedMessage.EventId)) is not {Value: { } value})
        {
            eventId = default;
            return false;
        }
        eventId = value.StringValue;
        return true;
    }

    public static bool TryGetFileProcessingId(this SQSEvent.SQSMessage message, out Guid fileProcessingId)
    {
        fileProcessingId = Guid.Empty;
        return message.MessageAttributes.SingleOrDefault(_=>_.Key == TenantBasedMessage.FileProcessingId) is {Value:{} value}  && Guid.TryParse(value.StringValue, out fileProcessingId);
    }


}