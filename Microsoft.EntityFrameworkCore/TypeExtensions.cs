using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.EntityFrameworkCore;

public static class TypeExtensions
{
    public static bool IsOwned(this Type type)
    {
        return type.GetCustomAttribute<OwnedAttribute>() != null;
    }
}