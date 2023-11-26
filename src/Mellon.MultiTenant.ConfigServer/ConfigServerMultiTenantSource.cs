using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.Placeholder;

namespace Mellon.MultiTenant.ConfigServer;

public class ConfigServerTenantSource(
    MultiTenantOptions multiTenantOptions,
    IHostEnvironment hostEnvironment) : ITenantConfigurationSource
{
    public IConfigurationBuilder AddSource(
        string tenant,
        IConfigurationBuilder builder)
    {
        builder
            .AddConfigServer(
                hostEnvironment.EnvironmentName,
                $"{multiTenantOptions.ApplicationName ?? hostEnvironment.ApplicationName}-{tenant}")
            .AddPlaceholderResolver();

        return builder;
    }
}
