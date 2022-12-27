using Mellon.MultiTenant.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Mellon.MultiTenant;

public class TenantConfiguration : IMultiTenantConfiguration
{
    private readonly TenantSettings _tenantSettings;

    public IConfiguration Configuration { get; }
    public TenantSettings TenantSettings => _tenantSettings;

    public TenantConfiguration(
        TenantSettings tenantSettings,
        IConfiguration configuration)
    {
        Configuration = tenantSettings.Configuration ?? configuration;
        
        _tenantSettings = tenantSettings;
    }

    public string this[string key] { get => Configuration[key]; set => Configuration[key] = value; }

    public IEnumerable<IConfigurationSection> GetChildren() => Configuration.GetChildren();

    public IChangeToken GetReloadToken() => Configuration.GetReloadToken();

    public IConfigurationSection GetSection(string key) => Configuration.GetSection(key);
}
