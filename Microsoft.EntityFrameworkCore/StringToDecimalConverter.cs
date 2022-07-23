using System.ComponentModel;
using System.Globalization;

namespace Microsoft.EntityFrameworkCore;

public class StringToDecimalConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(decimal);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(decimal))
        {
            return decimal.Parse(value.ToString());
        }

        return default;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string s)
        {
            return decimal.Parse(s);
        }

        return default;
    }
}