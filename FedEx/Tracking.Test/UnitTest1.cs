using System.Text.Json;
using FedEx.Tracking;

namespace Tracking.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var json = File.ReadAllText("data/response.json");
            TrkcResponseVO_TrackingNumber? x = System.Text.Json.JsonSerializer.Deserialize<TrkcResponseVO_TrackingNumber>(json, new JsonSerializerOptions(){Converters = { new LocationDetail1Converter() }});
        }
    }
}