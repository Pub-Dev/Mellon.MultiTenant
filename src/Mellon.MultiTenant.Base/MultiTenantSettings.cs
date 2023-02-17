using Mellon.MultiTenant.Base.Enums;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Mellon.MultiTenant.Base;

public class MultiTenantSettings
{
    private Dictionary<string, IConfigurationRoot> configurations = new Dictionary<string, IConfigurationRoot>();

    public IReadOnlyList<string> Tenants => configurations?.Keys.ToList();

    public IReadOnlyDictionary<string, IConfigurationRoot> GetConfigurations => configurations;

    public void LoadConfiguration(string tenant, IConfigurationRoot configuration)
    {
        if (!configurations.ContainsKey(tenant))
        {
            configurations[tenant] = configuration;
        }
        else
        {
            throw new Exception($"Tenant {tenant} already configured");
        }
    }

    public string[] LoadTenants(
        MultiTenantOptions multiTenantOptions,
        IConfiguration configuration)
    {
        switch (multiTenantOptions.TenantSource)
        {
            case TenantSource.EnvironmentVariables:
                if (configuration["MULTITENANT_TENANTS"] is null)
                {
                    throw new Exception($"MULTITENANT_TENANTS not set!");
                }
                return configuration["MULTITENANT_TENANTS"].Split(',', StringSplitOptions.RemoveEmptyEntries);

            case TenantSource.Settings:
                if (configuration.GetSection("MultiTenant:Tenants") is null)
                {
                    throw new Exception($"MultiTenant:Tenants not set!");
                }
                return configuration.GetSection("MultiTenant:Tenants").Get<string[]>();
            case TenantSource.Endpoint:
                var tenants = multiTenantOptions.GetTenantSourceHttpEndpointFunc(multiTenantOptions.Endpoint, configuration);
                if (tenants.Length == 0)
                {
                    throw new Exception($"No Tenants found on the external endpoint!");
                }
                return tenants;
            default:
                throw new Exception($"{nameof(multiTenantOptions.TenantSource)} not set!");
        }
    }

    public IConfigurationRoot BuildTenantConfiguration(
        IHostEnvironment hostEnvironment,
        ITenantConfigurationSource tenantConfigurationSource,
        MultiTenantOptions multiTenantOptions,
        string tenant)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true)
            .AddEnvironmentVariables();

        builder = tenantConfigurationSource.AddSource(tenant, builder);

        return builder.Build();
    }
}
