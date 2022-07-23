using System;

namespace YadaYada.Security.Entity;

public interface ITenantScoped
{
    public Guid TenantId { get; set; }
}