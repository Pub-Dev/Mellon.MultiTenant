using Microsoft.Extensions.Configuration;

namespace Mellon.MultiTenant.Interfaces;

public interface IMultiTenantConfiguration : IConfiguration
{
    TenantSettings TenantSettings { get; }
}
