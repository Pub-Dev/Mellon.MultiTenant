namespace Mellon.MultiTenant.Base.Interfaces;

using Microsoft.Extensions.Configuration;

public interface IMultiTenantConfigurationSource
{
	IConfigurationBuilder AddSource(string tenant, IConfigurationBuilder builder);
}