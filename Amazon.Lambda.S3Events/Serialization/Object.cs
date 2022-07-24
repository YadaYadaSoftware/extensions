using Newtonsoft.Json;

namespace YadaYada.Lambda.Events.S3;

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