using System;
using System.ComponentModel;
using System.Globalization;

namespace YadaYada.Data.Library;

public class StringToBoolConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(bool);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        var returnValue = false;
        switch (value?.ToString().ToUpperInvariant())
        {
            case "TRUE":
            case "1":
            case "YES":
                returnValue = true;
                break;
            case "FALSE":
            case "0":
            case "NO":
                returnValue = false;
                break;
            default:
                throw new InvalidOperationException();
        }


        return returnValue;
    }
}