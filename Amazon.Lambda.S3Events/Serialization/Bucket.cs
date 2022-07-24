using Newtonsoft.Json;

namespace Amazon.Lambda.S3Events.Serialization;

public partial class Bucket
{
    [JsonProperty("name")]
    public string Name { get; set; }
}