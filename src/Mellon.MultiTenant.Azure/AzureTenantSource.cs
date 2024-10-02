namespace Mellon.MultiTenant.Azure;

using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;

public class AzureTenantSource(
    IServiceProvider serviceProvider,
    AzureMultiTenantOptions azureMultiTenantOptions) : IMultiTenantConfigurationSource
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
                    .Select("*", tenant));
        }
        else
        {
            builder.AddAzureAppConfiguration(azureMultiTenantOptions.AzureAppConfigurationOptions(serviceProvider, tenant));
        }

        return builder;
    }
}
