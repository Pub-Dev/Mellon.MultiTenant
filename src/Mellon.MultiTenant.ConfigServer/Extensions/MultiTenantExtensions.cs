#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Mellon.MultiTenant.Base.Interfaces;
using Mellon.MultiTenant.ConfigServer;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class MultiTenantExtensions
{
	public static IServiceCollection AddMultiTenantSpringCloudConfig(
		this IServiceCollection services)
	{
		services.RemoveAll<IMultiTenantConfigurationSource>();

		services.AddSingleton<IMultiTenantConfigurationSource, ConfigServerMultiTenantSource>();

		return services;
	}
}