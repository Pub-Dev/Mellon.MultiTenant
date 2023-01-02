using Microsoft.Extensions.Configuration;

namespace Mellon.MultiTenant.Base.Interfaces;

public interface IMultiTenantSource
{
    IConfigurationBuilder AddSource(string tenant, IConfigurationBuilder builder);
}
