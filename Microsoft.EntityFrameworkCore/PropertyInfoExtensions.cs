using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore;

public static class PropertyInfoExtensions
{
    public static bool IsOwned(this PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.GetCustomAttribute<OwnedAttribute>() != default;
    }
    public static OwnedAttribute GetOwnedAttribute(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<OwnedAttribute>();
    }

    public static DynamicLinqSelectAttribute GetDynamicLinqSelect(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<DynamicLinqSelectAttribute>();
    }
}