using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Mellon.MultiTenant.Azure;

public class AzureTenantSource(
    IServiceProvider serviceProvider,
    AzureMultiTenantOptions azureMultiTenantOptions) : ITenantConfigurationSource
{
    public IConfigurationBuilder AddSource(
        string tenant,
        IConfigurationBuilder builder)
    {
        if (azureMultiTenantOptions.AzureAppConfigurationOptions is null)
        {
            if (azureMultiTenantOptions.AzureAppConfigurationConnectionString is null)
            {
                throw new Exception($"AzureAppConfigurationOptions is required when using Azure");
            }

            builder.AddAzureAppConfiguration(options =>
                options
                    .Connect(azureMultiTenantOptions.AzureAppConfigurationConnectionString)
                    .Select("*", tenant)
            );
        }
        else
        {
            builder.AddAzureAppConfiguration(azureMultiTenantOptions.AzureAppConfigurationOptions(serviceProvider, tenant));
        }

        return builder;
    }
}
