using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Amazon.SQS.Model;

namespace Amazon.Lambda.SQSEvents;


public class TenantBasedMessage : SendMessageRequest
{

    public const string DataTypeInt = "Number.int";
    public const string DataTypeGuid = "String.guid";
    public const string DataTypeType = "String.type";
    public const string FileProcessingId = "FileProcessingId";

    public TenantBasedMessage(string eventId, [NotNull] string body)
    {
        if (eventId == default) throw new ArgumentNullException(nameof(eventId));
        this.MessageGroupId = eventId.ToString();
        this.MessageBody = body ?? throw new ArgumentNullException(nameof(body));
        this.EventId = eventId;
    }

    protected TenantBasedMessage(string eventId, object body) : this((string) eventId, System.Text.Json.JsonSerializer.Serialize(body))
    {
    }

    [JsonIgnore]
    public Type MessageType
    {
        get => this.GetType();
        set => this.SetType(value);
    }

    [JsonIgnore]
    public Guid TenantId
    {
        get => this.GetGuid();
        set => this.SetGuid(value);
    }

    [JsonIgnore]
    public string EventId
    {
        get => this.GetString();
        private init => this.SetString(value);
    }

    [JsonIgnore]
    public Type Sender
    {
        get => this.GetType();
        set => this.SetType(value);
    }

    protected Type GetType([CallerMemberName] string caller = null)
    {
        Type returnValue = default;
        if (this.MessageAttributes.ContainsKey(caller))
        {
            returnValue = Type.GetType(MessageAttributes[caller].StringValue);
        }

        return returnValue;
    }
    protected void SetType(Type value, [CallerMemberName] string caller = null)
    {
        if (this.MessageAttributes.ContainsKey(caller))
        {
            if (!this.MessageAttributes[caller].StringValue.Equals(value.FullName)) throw new InvalidOperationException($"Cannot update {caller}");
            return;
        }

        var valueString = $"{value.FullName}, {value.Assembly.GetName().Name}, Culture=neutral, PublicKeyToken=null";

        this.MessageAttributes.Add(caller, new MessageAttributeValue { StringValue = valueString, DataType = DataTypeType });
    }

    protected Guid GetGuid([CallerMemberName] string caller = null)
    {
        Guid returnValue = default;
        if (this.MessageAttributes.ContainsKey(caller))
        {
            returnValue = Guid.Parse(MessageAttributes[caller].StringValue);
        }

        return returnValue;
    }
    protected void SetGuid(Guid value, [CallerMemberName] string caller = null)
    {
        if (this.MessageAttributes.ContainsKey(caller))
        {
            if (!this.MessageAttributes[caller].StringValue.Equals(value.ToString())) throw new InvalidOperationException($"Cannot update {caller}");
            return;
        }

        this.MessageAttributes.Add(caller, new MessageAttributeValue { StringValue = value.ToString(), DataType = DataTypeGuid });
    }
    protected int GetInt([CallerMemberName] string caller = null)
    {
        int returnValue = default;
        if (this.MessageAttributes.ContainsKey(caller))
        {
            returnValue = int.Parse(MessageAttributes[caller].StringValue);
        }

        return returnValue;
    }
    protected bool GetBool([CallerMemberName] string caller = null)
    {
        bool returnValue = default;
        if (this.MessageAttributes.ContainsKey(caller))
        {
            returnValue = bool.Parse(MessageAttributes[caller].StringValue);
        }

        return returnValue;
    }
    protected void SetInt(int value, [CallerMemberName] string caller = null)
    {
        if (this.MessageAttributes.ContainsKey(caller))
        {
            if (!this.MessageAttributes[caller].StringValue.Equals(value.ToString())) throw new InvalidOperationException($"Cannot update {caller}");
            return;
        }

        this.MessageAttributes.Add(caller, new MessageAttributeValue { StringValue = value.ToString(), DataType = DataTypeInt });
    }

    protected string GetString([CallerMemberName] string caller = null)
    {
        String returnValue = default;
        if (!this.MessageAttributes.ContainsKey(caller))
        {
            returnValue = MessageAttributes[caller].StringValue;
        }

        return returnValue;
    }
    protected void SetString(String value, [CallerMemberName] string caller = null)
    {
        if (this.MessageAttributes.ContainsKey(caller))
        {
            if (!this.MessageAttributes[caller].StringValue.Equals(value)) throw new InvalidOperationException($"Cannot update {caller}");
            return;
        }

        this.MessageAttributes.Add(caller, new MessageAttributeValue { StringValue = value.ToString(), DataType = "String" });
    }
    protected void SetBool(bool value, [CallerMemberName] string caller = null)
    {
        if (this.MessageAttributes.ContainsKey(caller))
        {
            if (!this.MessageAttributes[caller].StringValue.Equals(value.ToString())) throw new InvalidOperationException($"Cannot update {caller}");
            return;
        }

        this.MessageAttributes.Add(caller, new MessageAttributeValue { StringValue = value.ToString(), DataType = "String" });
    }
}

