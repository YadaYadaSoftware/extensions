using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;

namespace System.Text.Test.Json.Serialization;

public class FileInfoConverterTest
{
    [Fact]
    public void SerializeFileInfoTest()
    {
        var assemblyLocation = this.GetType().Assembly.Location;
        var t = new MyClassWithFileInfoProperty() { MyFileInfo = new FileInfo(assemblyLocation) };
        var s = JsonSerializer.Serialize(t);
        var t2 = JsonSerializer.Deserialize<MyClassWithFileInfoProperty>(s);
        t2.MyFileInfo.FullName.Should().Be(assemblyLocation);
    }

    public class MyClassWithFileInfoProperty
    {
        [System.Text.Json.Serialization.JsonConverter(typeof(FileInfoConverter))]
        public FileInfo MyFileInfo { get; set; }
    }

}