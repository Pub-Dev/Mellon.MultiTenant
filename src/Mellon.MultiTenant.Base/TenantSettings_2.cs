using Microsoft.Extensions.Configuration;

namespace Mellon.MultiTenant.Base;

public class TenantSettings2
{
    private readonly MultiTenantSettings _multiTenantSettings;

    public string Tenant { get; private set; }

    public IConfiguration Configuration => Tenant is null ? null : _multiTenantSettings.GetConfigurations[Tenant];

    public TenantSettings2(MultiTenantSettings multiTenantSettings)
    {
        _multiTenantSettings = multiTenantSettings;
    }

    public void SetCurrentTenant(string tenant)
    {
        if (_multiTenantSettings.GetConfigurations.ContainsKey(tenant))
        {
            Tenant = tenant;
        }
        else
        {
            throw new Exception($"Tenant {tenant} not found");
        }
    }
}
