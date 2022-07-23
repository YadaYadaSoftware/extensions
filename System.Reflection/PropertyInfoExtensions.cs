using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace System.Reflection;

public static class PropertyInfoExtensions
{

    public static bool IsAutoGenerateField(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetDisplayAttribute()?.GetAutoGenerateField() ?? false;
    }
    public static bool IsFilterable(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetDisplayAttribute()?.GetAutoGenerateFilter() ?? false;
    }
    public static bool IsSortable(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<NotMappedAttribute>() is null;
    }
    public static bool IsEditable(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<EditableAttribute>()?.AllowEdit ?? false;
    }
    public static ForeignKeyAttribute GetForeignKeyAttribute(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
    }

    public static bool IsGroupable(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<GroupableAttribute>() != null;
    }

    public static PropertyInfo GetForeignKeyProperty(this PropertyInfo propertyInfo)
    {
        PropertyInfo returnValue = null;
        var foreignKeyAttribute = propertyInfo.ReflectedType.GetMetadataType().GetProperty(propertyInfo.Name)?.GetForeignKeyAttribute();
        if (foreignKeyAttribute != null)
        {
            returnValue =  propertyInfo.ReflectedType.GetProperty(foreignKeyAttribute.Name);
        }

        if (returnValue == null)
        {
            foreignKeyAttribute = propertyInfo.ReflectedType.GetProperty(propertyInfo.Name)?.GetForeignKeyAttribute();
            if (foreignKeyAttribute != null)
            {
                returnValue = propertyInfo.ReflectedType.GetProperty(foreignKeyAttribute.Name);
            }

        }

        return returnValue;
    }

    public static string GetValueField(this PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.GetKeyProperty().Name;
    }

    public static string GetTextField(this PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.GetDisplayColumn().DisplayColumn;
    }
    public static object GetValueExpression(this PropertyInfo propertyInfo, object currentItem)
    {
        try
        {
            if (propertyInfo.GetForeignKeyAttribute() is { } foreignKeyAttribute)
            {
                propertyInfo = propertyInfo.GetForeignKeyProperty();
            }

            var createExpressionMethod = typeof(PropertyInfoExtensions).GetMethod(nameof(CreateExpression), BindingFlags.Public | BindingFlags.Static);
            var genericMethod = createExpressionMethod.MakeGenericMethod(propertyInfo.PropertyType);
            return genericMethod.Invoke(null, new object[] {propertyInfo, currentItem});
        }
        catch (Exception e)
        {
            throw new NotSupportedException($"Cannot determine {nameof(GetValueExpression)} for {propertyInfo.Name} given an object of type '{currentItem?.GetType().FullName}'.", e);
        }
    }

    public static Expression<Func<TValue>> CreateExpression<TValue>(PropertyInfo propertyInfo, object currentItem)
    {

        var constant = Expression.Constant(currentItem);
        var exp = Expression.Property(constant, propertyInfo.Name);
        return Expression.Lambda<Func<TValue>>(exp);
    }


    public static DisplayAttribute GetDisplayAttribute(this PropertyInfo propertyInfo)
    {
        var displayAttribute = propertyInfo.GetCustomAttribute<DisplayAttribute>();
        if (displayAttribute != null || propertyInfo.ReflectedType!.GetMetadataType() is not { } metadataType) return displayAttribute;
        var metadataPropertyInfo = metadataType.GetProperty(propertyInfo.Name);
        displayAttribute = metadataPropertyInfo?.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute;

    }


    public static string GetDisplayColumn(this PropertyInfo propertyInfo)
    {
        return propertyInfo.PropertyType.GetCustomAttribute<DisplayColumnAttribute>()?.DisplayColumn;

    }

    public static string GetKeyPropertyName(this PropertyInfo propertyInfo)
    {
        var keyAttributeProperty = propertyInfo.PropertyType.GetProperties().SingleOrDefault(info => info.GetCustomAttribute<KeyAttribute>() is { });
        if (keyAttributeProperty == default) throw new InvalidOperationException($"Cannot retrieve a {nameof(KeyAttribute)} for {propertyInfo.Name}.");
        return keyAttributeProperty.Name;

    }

    public static string GetGroupName(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetDisplayAttribute()?.GetGroupName();
    }
    public static string GetDescription(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetDisplayAttribute()?.GetDescription();
    }
    public static string GetPrompt(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetDisplayAttribute()?.GetPrompt();
    }
    public static int GetOrder(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetDisplayAttribute()?.GetOrder() ?? 0;
    }
    public static string GetName(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetDisplayAttribute()?.GetName() ?? propertyInfo.Name;
    }
    public static string GetShortName(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetDisplayAttribute()?.GetShortName();
    }
    public static string GetDisplayFormat(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<DisplayFormatAttribute>()?.DataFormatString ?? string.Empty;
    }
    public static RangeAttribute GetRange(this PropertyInfo propertyInfo)
    {
        var rangeAttribute = propertyInfo.GetCustomAttribute<RangeAttribute>();
        if (rangeAttribute == null 
            && propertyInfo.ReflectedType.GetMetadataType() is { } metadataType 
            && metadataType != propertyInfo.ReflectedType
            && metadataType.GetProperty(propertyInfo.Name) is {} metadataTypePropertyInfo)
        {
            rangeAttribute = metadataTypePropertyInfo.GetRange();
        }
        return rangeAttribute;
    }

    public static object GetMaximumRange(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttribute<RangeAttribute>() is { } range)
        {
            return range.Maximum;
        }

        return double.MaxValue;
    }
    public static int? GetMaximumLength(this PropertyInfo propertyInfo)
    {
        if (propertyInfo.GetCustomAttribute<StringLengthAttribute>() is { } stringLengthAttribute)
        {
            return stringLengthAttribute.MaximumLength;
        }

        if (propertyInfo.GetCustomAttribute<MaxLengthAttribute>() is { } maxLengthAttribute)
        {
            return maxLengthAttribute.Length;
        }

        return null;
    }

    public static int? GetMaxStringLengthOfDisplayColumn(this PropertyInfo propertyInfo)
    {
        var propertyType = propertyInfo?.PropertyType;
        var foreignKeyDisplayColumnProperty = propertyType?.GetDisplayColumnProperty();
        var maxStringLength = foreignKeyDisplayColumnProperty?.GetMaximumLength();
        return maxStringLength;
    }
}