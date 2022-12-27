using Azure.Identity;
using Mellon.MultiTenant.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.Placeholder;

namespace Mellon.MultiTenant;
public class MultiTenantSettings
{
    Dictionary<string, IConfiguration> Configurations = new Dictionary<string, IConfiguration>();

    public List<string> Tenants { get; private set; } = new List<string>();

    public void LoadConfiguration(string tenant, IConfiguration configuration)
    {
        if (!Configurations.ContainsKey(tenant))
        {
            Configurations[tenant] = configuration;

            Tenants.Add(tenant);
        }
        else
        {
            throw new Exception($"Tenant {tenant} already configured");
        }
    }

    public Dictionary<string, IConfiguration> GetConfigurations => Configurations;

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
        MultiTenantOptions multiTenantOptions,
        IHostEnvironment hostEnvironment,
        string tenant)
    {
        var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true)
                    .AddJsonFile($"appsettings.{hostEnvironment.EnvironmentName}.json", true);

        switch (multiTenantOptions.ConfigurationSource)
        {
            case ConfigurationSource.Azure:
                var credentials = new ManagedIdentityCredential();

                builder.AddAzureAppConfiguration(options =>
                {
                    options.Connect(new Uri(Environment.GetEnvironmentVariable("AzureAppConfigurationEndPointURL")), credentials)
                        .ConfigureKeyVault(kv => { kv.SetCredential(credentials); })
                        .Select("*", tenant);
                });
                break;
            case ConfigurationSource.SpringCloud:
                builder
                    .AddConfigServer(
                        hostEnvironment.EnvironmentName,
                        $"{multiTenantOptions.ApplicationName ?? hostEnvironment.ApplicationName}-{tenant}")
                    .AddPlaceholderResolver();
                break;
            case ConfigurationSource.Local:
                builder.AddJsonFile($"appsettings.{tenant}.json", true);
                builder.AddJsonFile($"appsettings.{tenant}.{hostEnvironment.EnvironmentName}.json", true);
                break;
            default:
                break;
        }

        return builder.Build();
    }
}
