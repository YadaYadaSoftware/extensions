namespace Microsoft.EntityFrameworkCore;

public interface ITenantScoped
{
    public Guid TenantId { get; set; }
}