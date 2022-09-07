using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FedEx.Tracking;

public class LocationDetail1Converter : JsonConverter<LocationDetail_1>
{
    public override LocationDetail_1? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        LocationDetail_1 returnValue = new LocationDetail_1();

        if (reader.TokenType != JsonTokenType.StartObject) throw new InvalidOperationException();

        var startObject = 1;
        var endObject = 0;

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.StartObject:
                    startObject++;
                    break;
                case JsonTokenType.EndObject:
                    endObject++;
                    break;
                case JsonTokenType.PropertyName:
                    var propertyName = reader.GetString();
                    switch (propertyName)
                    {
                        case nameof(LocationDetail_1.LocationType):
                            returnValue.LocationType = LocationDetail_1LocationType.FEDEX_AUTHORIZED_SHIP_CENTER;
                            break;
                        default:
                            Debug.WriteLine(propertyName);
                            break;
                    }
                    break;
            }

            if (startObject == endObject)
            {
                break;
            }
        }

        return returnValue;
    }

    public override void Write(Utf8JsonWriter writer, LocationDetail_1 value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}