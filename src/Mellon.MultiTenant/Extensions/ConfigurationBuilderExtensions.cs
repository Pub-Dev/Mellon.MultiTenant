using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.Placeholder;

namespace Mellon.MultiTenant.Extensions
{
    internal static class ConfigurationBuilderExtensions
    {
        internal static void AddAzure(
            this IConfigurationBuilder builder,
            MultiTenantOptions multiTenantOptions,
            string tenant)
        {
            if (multiTenantOptions.AzureAppConfigurationOptions is null)
            {
                if (multiTenantOptions.AzureAppConfigurationConnectionString is null)
                {
                    throw new Exception($"AzureAppConfigurationOptions is required when using Azure");
                }

                builder.AddAzureAppConfiguration(options =>
                    options
                        .Connect(multiTenantOptions.AzureAppConfigurationConnectionString)
                        .Select("*", tenant)
                );
            }
            else
            {
                builder.AddAzureAppConfiguration(multiTenantOptions.AzureAppConfigurationOptions(tenant));
            }
        }

        internal static IConfigurationBuilder AddSpringCloudConfig(
            this IConfigurationBuilder builder,
            IHostEnvironment hostEnvironment,
            MultiTenantOptions multiTenantOptions,
            string tenant)
        {
            builder
                .AddConfigServer(
                    hostEnvironment.EnvironmentName,
                    $"{multiTenantOptions.ApplicationName ?? hostEnvironment.ApplicationName}-{tenant}")
                .AddPlaceholderResolver();

            return builder;
        }

        internal static IConfigurationBuilder AddLocalConfig(
            this IConfigurationBuilder builder,
            IHostEnvironment hostEnvironment,
            string tenant)
        {
            builder.AddJsonFile($"appsettings.{tenant}.json", true);
            builder.AddJsonFile($"appsettings.{tenant}.{hostEnvironment.EnvironmentName}.json", true);

            return builder;
        }
    }
}
