using Mellon.MultiTenant.Base.Enums;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Mellon.MultiTenant.Base;

public class MultiTenantSettings
{
    private Dictionary<string, IConfigurationRoot> configurations = new Dictionary<string, IConfigurationRoot>();

    public List<string> Tenants => configurations?.Keys.ToList();

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

    public IReadOnlyDictionary<string, IConfigurationRoot> GetConfigurations => configurations;

    public string[] LoadTenants(
        MultiTenantOptions multiTenantOptions,
        IConfiguration configuration)
    {
        switch (multiTenantOptions.TenantSource)
        {
            case TenantSource.EnvironmentVariables:
                return configuration["MULTITENANT_TENANTS"].Split(',', StringSplitOptions.RemoveEmptyEntries);

            case TenantSource.AppSettings:
                return configuration.GetSection("MultiTenant:Tenants").Get<string[]>();

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
