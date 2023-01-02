using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Mellon.MultiTenant
{
    public class LocalTenantSource : ITenantConfigurationSource
    {
        private readonly IHostEnvironment _hostEnvironment;

        public LocalTenantSource(
            IHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public IConfigurationBuilder AddSource(
            string tenant,
            IConfigurationBuilder builder)
        {
            builder.AddJsonFile($"appsettings.{tenant}.json", true);
            builder.AddJsonFile($"appsettings.{tenant}.{_hostEnvironment.EnvironmentName}.json", true);

            return builder;
        }
    }
}
