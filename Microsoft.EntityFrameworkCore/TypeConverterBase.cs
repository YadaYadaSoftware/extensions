using System.ComponentModel;

namespace Microsoft.EntityFrameworkCore;

public abstract class TypeConverterBase : TypeConverter
{
    private readonly Type _canConvertTo;
    private readonly Type[] _canConvertFromTypes;

    protected TypeConverterBase(Type canConvertTo, params Type[] canConvertFromTypes)
    {
        _canConvertTo = canConvertTo;
        _canConvertFromTypes = canConvertFromTypes;
    }

    // ReSharper disable once StaticMemberInGenericType
    public static DbContext Context { get; set; }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return _canConvertFromTypes.Contains(sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return _canConvertTo == destinationType;
    }
}