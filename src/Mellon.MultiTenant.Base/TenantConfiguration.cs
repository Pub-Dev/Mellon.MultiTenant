using Mellon.MultiTenant.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Mellon.MultiTenant.Base;

public class TenantConfiguration : IMultiTenantConfiguration
{
    public string Tenant { get; }

    public IConfiguration Configuration { get; }

    public TenantConfiguration(
        TenantSettings tenantSettings,
        IConfiguration configuration)
    {
        Tenant = tenantSettings.Tenant;
        Configuration = tenantSettings.Configuration ?? configuration;
    }

    public string this[string key] { get => Configuration[key]; set => Configuration[key] = value; }

    public IEnumerable<IConfigurationSection> GetChildren() => Configuration.GetChildren();

    public IChangeToken GetReloadToken() => Configuration.GetReloadToken();

    public IConfigurationSection GetSection(string key) => Configuration.GetSection(key);
}
