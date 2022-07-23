using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore;

public abstract class EntityTypeConfiguration
{

}
public abstract class EntityTypeConfiguration<T> : EntityTypeConfiguration, IEntityTypeConfiguration<T> where T : class
{

    protected EntityTypeConfiguration()
    {
            
    }

    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        if(typeof(T).GetCustomAttribute<TableAttribute>() is { } tableAttribute)
        {
            builder.ToTable(tableAttribute.Name, tableAttribute.Schema);
        }
        else
        {
            var entityType = builder.GetType().GenericTypeArguments[0];
            // ReSharper disable once PossibleNullReferenceException
            builder.ToTable(entityType.Name, entityType.Namespace.Split('.').First());
        }

        foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var unique = propertyInfo.GetCustomAttribute<UniqueAttribute>();
            if (unique == null) continue;
            builder.HasIndex(propertyInfo.Name).IsUnique();
        }

        foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attribute = propertyInfo.GetCustomAttribute<HasDefaultSqlValueAttribute>();
            if (attribute == null) continue;
            builder.Property(propertyInfo.Name).HasDefaultValueSql(attribute.Value);
        }

        var idProperty = typeof(T).GetProperty(nameof(IId.Id));
        if (idProperty != null && idProperty.PropertyType.IsAssignableFrom(typeof(Guid)))
        {
            builder.Property(nameof(IId.Id)).HasDefaultValueSql("(newsequentialid())");
        }
    }
}