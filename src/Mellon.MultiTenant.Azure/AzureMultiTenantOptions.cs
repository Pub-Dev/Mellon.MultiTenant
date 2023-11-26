using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Mellon.MultiTenant.Azure;

public class AzureMultiTenantOptions
{
    public string AzureAppConfigurationConnectionString { get; set; }

    public Func<IServiceProvider, string, Action<AzureAppConfigurationOptions>> AzureAppConfigurationOptions { get; set; }
}
