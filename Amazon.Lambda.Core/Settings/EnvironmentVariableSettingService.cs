using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class EnvironmentVariableSettingService : ISettingService
{
    static EnvironmentVariableSettingService()
    {
        TypeDescriptor.AddAttributes(typeof(Uri), new TypeConverterAttribute(typeof(UriConverter)));

    }
    public async Task<string> GetAsync([CallerMemberName] string name = "")
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
        var raw = GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(raw)) throw new KeyNotFoundException(name);

        try
        {

            return raw;

        }
        catch (Exception e)
        {

            throw new InvalidCastException(e.Message,e);
        }        }

    private class UriConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
            // ReSharper disable once AssignNullToNotNullAttribute
            return new Uri(value.ToString());
        }
    }

    internal virtual string GetEnvironmentVariable(string settingName)
    {
        return Environment.GetEnvironmentVariable(settingName);
    }
}