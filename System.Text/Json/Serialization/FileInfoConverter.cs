using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Text.Json.Serialization;

public class FileInfoConverter : Serialization.JsonConverter<FileInfo>
{
    public override FileInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        return new FileInfo(value);
    }

    public override void Write(Utf8JsonWriter writer, FileInfo value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.FullName);
    }
}

