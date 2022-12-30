using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Mellon.MultiTenant.Azure
{
    public class AzureMultiTenantSource : IMultiTenantSource
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AzureMultiTenantOptions _azureMultiTenantOptions;

        public AzureMultiTenantSource(
            IServiceProvider serviceProvider,
            AzureMultiTenantOptions azureMultiTenantOptions)
        {
            _serviceProvider = serviceProvider;

            _azureMultiTenantOptions = azureMultiTenantOptions;
        }

        public IConfigurationBuilder AddSource(
            string tenant,
            IConfigurationBuilder builder)
        {
            if (_azureMultiTenantOptions.AzureAppConfigurationOptions is null)
            {
                if (_azureMultiTenantOptions.AzureAppConfigurationConnectionString is null)
                {
                    throw new Exception($"AzureAppConfigurationOptions is required when using Azure");
                }

                builder.AddAzureAppConfiguration(options =>
                    options
                        .Connect(_azureMultiTenantOptions.AzureAppConfigurationConnectionString)
                        .Select("*", tenant)
                );
            }
            else
            {
                builder.AddAzureAppConfiguration(_azureMultiTenantOptions.AzureAppConfigurationOptions(_serviceProvider, tenant));
            }

            return builder;
        }
    }
}
