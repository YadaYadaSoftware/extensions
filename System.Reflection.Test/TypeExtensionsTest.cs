using System.ComponentModel.DataAnnotations;
using FluentAssertions;

namespace System.Reflection;

public class TypeExtensionsTest
{
    [Fact]
    public void GetGroupNamesTest()
    {
        var groupNames = typeof(SampleEntity).GetGroupNames();
        groupNames.First().Should().Be("Group 1");
        groupNames.Last().Should().Be("Group 2");

    }

    [Fact]
    public void HasPropertyTest()
    {
        var t = typeof(SampleEntity);
        t.HasProperty(nameof(SampleEntity.Name)).Should().BeTrue();
    }


}

[Display(
    ResourceType = typeof(Properties.SampleEntity.Class), 
    GroupName = nameof(DisplayAttribute.GroupName))]
public class SampleEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}