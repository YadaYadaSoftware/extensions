using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentAssertions;

namespace System.Reflection;

public class PropertyInfoExtensionsTest
{
    public class MyClassForEditableTesting
    {
        [Editable(true)]
        public string AlwaysEditableProperty { get; set; }

        [Editable(false, AllowInitialValue = true)]
        public string InitiallyEditableProperty { get; set; }
    }

    [DisplayColumn(nameof(Name))]
    public class MyForeignEntity
    {
        [StringLength(80)]
        public string Name { get; set; }
    }

    public class MyEntity
    {
        public Guid ForeignEntityId { get; set; }

        [ForeignKey(nameof(ForeignEntityId))]
        public MyForeignEntity ForeignEntity {get; set; }
    }

    [Fact]
    public void IsEditable()
    {
        var editableTesting = new MyClassForEditableTesting();
        var type = typeof(MyClassForEditableTesting);

        type.GetProperty(nameof(MyClassForEditableTesting.AlwaysEditableProperty))
            .IsEditable().Should().BeTrue();
        //type.GetProperty(nameof(MyClassForEditableTesting.InitiallyEditableProperty))
        //    .IsEditable().Should().BeTrue();

        editableTesting.AlwaysEditableProperty = Guid.NewGuid().ToString();
        editableTesting.InitiallyEditableProperty = Guid.NewGuid().ToString();


        type.GetProperty(nameof(MyClassForEditableTesting.AlwaysEditableProperty))
            .IsEditable().Should().BeTrue();
        //type.GetProperty(nameof(MyClassForEditableTesting.InitiallyEditableProperty))
        //    .IsEditable().Should().BeFalse();

    }

    [Fact]
    public void GetMaxStringLengthOfDisplayColumnTest()
    {
        var maxStringLength = typeof(MyEntity).GetProperty(nameof(MyEntity.ForeignEntity)).GetMaxStringLengthOfDisplayColumn();
        maxStringLength.Should().Be(80);
    }
}