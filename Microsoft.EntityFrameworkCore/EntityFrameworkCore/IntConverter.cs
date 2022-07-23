using System;
using System.ComponentModel;
using System.Globalization;

namespace YadaYada.Data.Library;

public class IntConverter : TypeConverter
{
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
        return destinationType == typeof(int) || destinationType == typeof(int?);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        return sourceType == typeof(string) || sourceType == typeof(int) || sourceType == typeof(int?) ||
               sourceType == typeof(decimal) || sourceType == typeof(decimal?) ||
               sourceType == typeof(double) || sourceType == typeof(double?);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string)
        {
            value = value.ToString()?.Replace("+", string.Empty);
            if (int.TryParse(value?.ToString(), out var returnValue))
            {
                return returnValue;
            }
        }

        return null;
    }
}