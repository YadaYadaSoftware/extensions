using System.ComponentModel;
using System.Runtime.Serialization;
using FluentAssertions;
using Xunit;

namespace YadaYada.Library.Extensions;

public class EnumHelperTest
{
    public enum TestEnum
    {
        [Description("Qqq"), EnumMember(Value = "Horse")] Mno = 1,
        [Description("Abc"), EnumMember(Value = "Zebra")] Xyz = 2

    }
    [Fact]
    public void ToStringFromDescriptionTest()
    {
        Assert.Equal("Abc", TestEnum.Xyz.ToStringFromDescription());
    }

    [Fact]
    public void ToEnumFromStringOfEnumMemberValue()
    {
        TestEnum x = "Zebra".FromEnumMemberValue<TestEnum>();
        x.Should().Be(TestEnum.Xyz);
        "Horse".FromEnumMemberValue<TestEnum>().Should().Be(TestEnum.Mno);
    }
}