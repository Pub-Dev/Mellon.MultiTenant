namespace Mellon.MultiTenant;

using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public class LocalMultiTenantSource(IHostEnvironment hostEnvironment) : IMultiTenantConfigurationSource
{
	public IConfigurationBuilder AddSource(
		string tenant,
		IConfigurationBuilder builder)
	{
		builder.AddJsonFile($"appsettings.{tenant}.json", optional: true);
		builder.AddJsonFile($"appsettings.{tenant}.{hostEnvironment.EnvironmentName}.json", optional: true);

		return builder;
	}
}
