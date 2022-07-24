using Newtonsoft.Json;

namespace Amazon.Lambda.S3Events.Serialization;

public partial class S3EventDetail
{
    [JsonProperty("version")]
    [JsonConverter(typeof(ParseStringConverter))]
    public long Version { get; set; }

    [JsonProperty("bucket")]
    public Bucket Bucket { get; set; }

    [JsonProperty("object")]
    public Object Object { get; set; }

    [JsonProperty("request-id")]
    public string RequestId { get; set; }

    [JsonProperty("requester")]
    public string Requester { get; set; }

    [JsonProperty("source-ip-address")]
    public string SourceIpAddress { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; set; }
}