namespace Mellon.MultiTenant.Azure;

using Microsoft.Extensions.Configuration.AzureAppConfiguration;

public class AzureMultiTenantOptions
{
	public string AzureAppConfigurationConnectionString { get; set; }

	public Func<IServiceProvider, string, Action<AzureAppConfigurationOptions>> AzureAppConfigurationOptions { get; set; }
}