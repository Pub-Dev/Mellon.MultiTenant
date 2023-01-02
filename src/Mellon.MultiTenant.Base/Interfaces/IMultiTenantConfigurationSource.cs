using Microsoft.Extensions.Configuration;

namespace Mellon.MultiTenant.Base.Interfaces;

public interface ITenantConfigurationSource
{
    IConfigurationBuilder AddSource(string tenant, IConfigurationBuilder builder);
}
