using Mellon.MultiTenant.Base.Enums;
using Mellon.MultiTenant.Base.Exceptions;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Mellon.MultiTenant.Base;

public class MultiTenantSettings
{
    private readonly Dictionary<string, IConfigurationRoot> configurations = [];

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

    public static string[] LoadTenants(
        MultiTenantOptions multiTenantOptions,
        IConfiguration configuration)
    {
        switch (multiTenantOptions.TenantSource)
        {
            case TenantSource.EnvironmentVariables:
                if (configuration["MULTITENANT_TENANTS"] is null)
                {
                    throw new TenantSourceNotSetException(TenantSource.EnvironmentVariables);
                }
                return configuration["MULTITENANT_TENANTS"].Split(',', StringSplitOptions.RemoveEmptyEntries);

            case TenantSource.Settings:
                if (configuration.GetSection("MultiTenant:Tenants") is null)
                {
                    throw new TenantSourceNotSetException(TenantSource.Settings);
                }
                return configuration.GetSection("MultiTenant:Tenants").Get<string[]>();
            case TenantSource.Endpoint:
                var tenants = multiTenantOptions.GetTenantSourceHttpEndpointFunc(multiTenantOptions.Endpoint, configuration);
                if (tenants.Length == 0)
                {
                    throw new TenantSourceNotSetException(TenantSource.Endpoint);
                }
                return tenants;
            default:
                throw new TenantSourceNotSetException(multiTenantOptions.TenantSource);
        }
    }

    public static IConfigurationRoot BuildTenantConfiguration(
        IHostEnvironment hostEnvironment,
        ITenantConfigurationSource tenantConfigurationSource,
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
