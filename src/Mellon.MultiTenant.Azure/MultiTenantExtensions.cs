using Mellon.MultiTenant.Azure;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Mellon.MultiTenant.Extensions
{
    public static class MultiTenantExtensions
    {
        public static IServiceCollection AddMultiTenantAzureAppConfiguration(
            this IServiceCollection services,
            Action<AzureMultiTenantOptions> action = null)
        {
            services.AddSingleton<AzureMultiTenantOptions>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                var azureMultiTenantOptions = new AzureMultiTenantOptions();

                azureMultiTenantOptions.AzureAppConfigurationConnectionString = configuration["AzureAppConfigurationConnectionString"];

                action?.Invoke(azureMultiTenantOptions);

                return azureMultiTenantOptions;
            });

            services.RemoveAll<ITenantConfigurationSource>();

            services.AddSingleton<ITenantConfigurationSource, AzureTenantSource>();

            return services;
        }
    }
}
