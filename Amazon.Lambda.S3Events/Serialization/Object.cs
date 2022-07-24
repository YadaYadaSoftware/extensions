using Newtonsoft.Json;

namespace Amazon.Lambda.S3Events.Serialization;

public partial class Object
{
    [JsonProperty("key")]
    public string Key { get; set; }

    [JsonProperty("size")]
    public long Size { get; set; }

    [JsonProperty("etag")]
    public string Etag { get; set; }

    [JsonProperty("sequencer")]
    public string Sequencer { get; set; }
}