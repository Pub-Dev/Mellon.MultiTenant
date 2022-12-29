using Mellon.MultiTenant.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Mellon.MultiTenant;

public class TenantConfiguration : IMultiTenantConfiguration
{
    public string Tenant { get; }
    public IConfiguration Configuration { get; }

    public TenantConfiguration(
        TenantSettings tenantSettings)
    {
        Tenant = tenantSettings.Tenant;

        Configuration = tenantSettings.Configuration;
    }

    public string this[string key] { get => Configuration[key]; set => Configuration[key] = value; }

    public IEnumerable<IConfigurationSection> GetChildren() => Configuration.GetChildren();

    public IChangeToken GetReloadToken() => Configuration.GetReloadToken();

    public IConfigurationSection GetSection(string key) => Configuration.GetSection(key);
}
