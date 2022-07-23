#region usings

using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

#endregion

namespace System;

public static class EnumHelper
{
    public static string ToStringFromDescription<T>(this T value) where T : Enum
    {
        MemberInfo[] memberInfos = value.GetType().GetMembers(BindingFlags.Public | BindingFlags.Static);
        var theMember = memberInfos.Single(r => r.Name.ToString() == value.ToString());
        var theEnumMemberAttribute = theMember.GetCustomAttribute<DescriptionAttribute>();
        return theEnumMemberAttribute?.Description;
    }
    public static string GetEnumMemberValue<T>(this T enumValue) where T : Enum
    {
        MemberInfo[] memberInfos = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Static);
        var theMember = memberInfos.Single(r => r.Name.ToString() == enumValue.ToString());
        var theEnumMemberAttribute = theMember.GetCustomAttributes(typeof(EnumMemberAttribute), true).First() as EnumMemberAttribute;
        return theEnumMemberAttribute.Value;
    }

    public static T FromEnumMemberValue<T>(this string value) where T : Enum
    {
        var enumMember = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Static).SingleOrDefault(m =>
            m.GetCustomAttribute<EnumMemberAttribute>() != null &&
            m.GetCustomAttribute<EnumMemberAttribute>().Value == value);

        return enumMember != null ? (T)Enum.Parse(typeof(T), enumMember.Name) : default;
    }
}