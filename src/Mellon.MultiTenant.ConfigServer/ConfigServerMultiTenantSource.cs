using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.Placeholder;

namespace Mellon.MultiTenant.ConfigServer
{
    public class ConfigServerTenantSource : ITenantConfigurationSource
    {
        private readonly MultiTenantOptions _multiTenantOptions;
        private readonly IHostEnvironment _hostEnvironment;

        public ConfigServerTenantSource(
            MultiTenantOptions multiTenantOptions,
            IHostEnvironment hostEnvironment)
        {
            _multiTenantOptions = multiTenantOptions;
            _hostEnvironment = hostEnvironment;
        }

        public IConfigurationBuilder AddSource(
            string tenant,
            IConfigurationBuilder builder)
        {
            builder
                .AddConfigServer(
                    _hostEnvironment.EnvironmentName,
                    $"{_multiTenantOptions.ApplicationName ?? _hostEnvironment.ApplicationName}-{tenant}")
                .AddPlaceholderResolver();

            return builder;
        }
    }
}
