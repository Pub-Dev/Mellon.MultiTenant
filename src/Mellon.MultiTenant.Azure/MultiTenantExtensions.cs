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
            Action<AzureMultiTenantOptions> action)
        {
            services.AddSingleton<AzureMultiTenantOptions>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                var azureMultiTenantOptions = new AzureMultiTenantOptions();

                azureMultiTenantOptions.AzureAppConfigurationConnectionString = configuration["AzureAppConfigurationConnectionString"];

                action?.Invoke(azureMultiTenantOptions);

                return azureMultiTenantOptions;
            });

            services.RemoveAll<IMultiTenantSource>();

            services.AddSingleton<IMultiTenantSource, AzureMultiTenantSource>();

            return services;
        }

        public static IServiceCollection AddMultiTenantAzureAppConfiguration(
            this IServiceCollection services) => services.AddMultiTenantAzureAppConfiguration(null);
    }
}
