using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.SqlClient;
using YadaYada.Security.Entity;

[assembly: InternalsVisibleTo("YadaYada.Data.Library.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace YadaYada.Data.Library;

public abstract class ContextBase : DbContext
{
    [NotNull] protected internal readonly IEnumerable<Claim> Claims;
    private readonly IEnumerable<EntityTypeConfiguration> _entityTypeConfigurations;
    protected readonly ILogger Logger;

    protected ContextBase([NotNull] DbContextOptions options, 
        [NotNull] ILoggerProvider loggerProvider,
        [NotNull] IEnumerable<EntityTypeConfiguration> entityTypeConfigurations, 
        IEnumerable<Claim> claims,
        IOptions<SqlConnectionStringBuilder> sqlConnectionStringBuilderOptions)
        : base(options)
    {
        Logger = loggerProvider.CreateLogger(this.GetType().FullName!);

        Claims = claims;
        
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (loggerProvider == null) throw new ArgumentNullException(nameof(loggerProvider));

        var typeConfigurations = entityTypeConfigurations as EntityTypeConfiguration[] ?? entityTypeConfigurations.ToArray();

        if (entityTypeConfigurations == null || !typeConfigurations.Any())
        {
            throw new ArgumentNullException(nameof(entityTypeConfigurations));
        }

        _entityTypeConfigurations = typeConfigurations;
        _sqlConnectionStringBuilder = sqlConnectionStringBuilderOptions?.Value;
    }

    public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
    {
        return this.Database.BeginTransaction(isolationLevel);
    }

    public override int SaveChanges()
    {
        CheckAndUpdateTenant();
        ValidateEntities();
        return base.SaveChanges();
    }

    protected virtual void CheckAndUpdateTenant()
    {
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        CheckAndUpdateTenant();
        ValidateEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }


    private void ValidateEntities()
    {
        var entityEntities = from e in ChangeTracker.Entries()
            where e.State == EntityState.Added
                  || e.State == EntityState.Modified
            select e;
        foreach (var entity in entityEntities)
        {
            if (entity.Entity is ITrackChanges trackChanges)
            {
                trackChanges.Updated = DateTime.Now;
                if (entity.State == EntityState.Added)
                {
                    trackChanges.Created = trackChanges.Updated;
                }
            }
        }
        var entities = from e in ChangeTracker.Entries()
            where e.State == EntityState.Added
                  || e.State == EntityState.Modified
            select e.Entity;

        var message = new StringBuilder();

        foreach (var entity in entities)
        {
            var validationContext = new ValidationContext(entity);

            try
            {
                Validator.ValidateObject(entity, validationContext,true);
            }
            catch (ValidationException e)
            {
                foreach (var validationResultMemberName in e.ValidationResult.MemberNames)
                {
                    message.AppendLine($"{entity.GetType().FullName}.{validationResultMemberName}: {e.Message}");
                }

                try
                {
                    message.AppendLine(System.Text.Json.JsonSerializer.Serialize(entity, new JsonSerializerOptions(){ReferenceHandler = ReferenceHandler.IgnoreCycles} ));
                }
                catch
                {
                    // ignored
                }
            }
        }

        if (message.Length > 0)
        {
            throw new ValidationException(message.ToString());

        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        CheckAndUpdateTenant();
        ValidateEntities();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
    {
        CheckAndUpdateTenant();
        this.ValidateEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private readonly SqlConnectionStringBuilder _sqlConnectionStringBuilder;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        using (Logger.BeginScope(nameof(OnConfiguring)))
        using (Logger.AddScope(nameof(optionsBuilder.IsConfigured), optionsBuilder.IsConfigured))
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.EnableSensitiveDataLogging();

            if (Claims?.Any() ?? false) optionsBuilder.ReplaceService<IModelCacheKeyFactory, ClaimsPrincipalModelKeyFactory>();
            if (optionsBuilder.IsConfigured) return;

            if (_sqlConnectionStringBuilder == null) throw new ArgumentNullException(nameof(_sqlConnectionStringBuilder));

            if (string.IsNullOrEmpty(_sqlConnectionStringBuilder.Password))
            {
                throw new InvalidOperationException($"Password is missing:{_sqlConnectionStringBuilder.ConnectionString}");
            }
            if (string.IsNullOrEmpty(_sqlConnectionStringBuilder.InitialCatalog))
            {
                var passwordMasked = _sqlConnectionStringBuilder.ConnectionString.Replace(_sqlConnectionStringBuilder.Password, "********");
                throw new InvalidOperationException($"InitialCatalog is missing :{passwordMasked}");
            }

            //Logger.LogInformation($"{nameof(_sqlConnectionStringBuilder.ConnectionString)}='{_sqlConnectionStringBuilder.ConnectionString}'");

            optionsBuilder.UseSqlServer(_sqlConnectionStringBuilder.ConnectionString, b => b.MigrationsAssembly("Data.Migrations"));
        }
    }

    protected void ApplyConfigurations(ModelBuilder modelBuilder)
    {
        foreach (var entityTypeConfiguration in _entityTypeConfigurations)
        {
            var interfaces = entityTypeConfiguration.GetType().GetInterfaces();
            var interfaceType = interfaces.Single(_ =>
                _.Name.StartsWith(nameof(IEntityTypeConfiguration<object>)));
            var entityType = interfaceType.GetGenericArguments().Single();
            var applyConfiguration = typeof(ModelBuilder).GetMethod(nameof(ModelBuilder.ApplyConfiguration))
                .MakeGenericMethod(entityType);

            applyConfiguration.Invoke(modelBuilder, new object[] { entityTypeConfiguration });
        }

        base.OnModelCreating(modelBuilder);

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplyConfigurations(modelBuilder);

        if (Claims?.TryGetTenantId(out Guid tenantId) ?? false)
        {
            var tenantEntityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(_ =>
                    typeof(ITenantScoped).IsAssignableFrom(_.ClrType)
                    && _.BaseType == null);

            foreach (var mutableEntityType in tenantEntityTypes)
            {
                var tenantFilter = (Expression<Func<ITenantScoped, bool>>)(e => e.TenantId == tenantId);
                modelBuilder.Entity(mutableEntityType.ClrType).AddQueryFilter(tenantFilter);
            }
        }
        

        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal)))
        {
            new PropertyBuilder(property).HasPrecision(18, 2);
        }
    }
}