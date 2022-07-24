using Newtonsoft.Json;

namespace YadaYada.Lambda.Events.S3;

public partial class Bucket
{
    [JsonProperty("name")]
    public string Name { get; set; }
}