namespace Mellon.MultiTenant.ConfigServer;

using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.Placeholder;

public class ConfigServerMultiTenantSource(
	MultiTenantOptions multiTenantOptions,
	IHostEnvironment hostEnvironment) : IMultiTenantConfigurationSource
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