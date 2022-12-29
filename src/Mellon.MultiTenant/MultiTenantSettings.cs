using Mellon.MultiTenant.Enums;
using Mellon.MultiTenant.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Mellon.MultiTenant;
public class MultiTenantSettings
{
    Dictionary<string, IConfigurationRoot> configurations = new Dictionary<string, IConfigurationRoot>();

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
                return Environment.GetEnvironmentVariable("MULTITENANT_TENANTS").Split(',', StringSplitOptions.RemoveEmptyEntries);
            case TenantSource.AppSettings:
                return configuration.GetSection("MultiTenant:Tenants").Get<string[]>();
            default:
                throw new Exception($"{nameof(multiTenantOptions.TenantSource)} not set!");
        }
    }

    public IConfigurationRoot BuildTenantConfiguration(
        IHostEnvironment hostEnvironment,
        MultiTenantOptions multiTenantOptions,
        string tenant)
    {
        var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true)
                    .AddEnvironmentVariables();

        switch (multiTenantOptions.ConfigurationSource)
        {
            case ConfigurationSource.Azure:
                builder.AddAzure(multiTenantOptions, tenant);
                break;
            case ConfigurationSource.SpringCloud:
                builder.AddSpringCloudConfig(hostEnvironment, multiTenantOptions, tenant);
                break;
            case ConfigurationSource.Local:
                builder.AddLocalConfig(hostEnvironment, tenant);
                break;
            default:
                break;
        }

        return builder.Build();
    }


}
