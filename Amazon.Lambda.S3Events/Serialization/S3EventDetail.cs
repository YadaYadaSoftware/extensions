using Newtonsoft.Json;

namespace YadaYada.Lambda.Events.S3;

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