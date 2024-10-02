#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Mellon.MultiTenant.Azure;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class MultiTenantExtensions
{
	public static IServiceCollection AddMultiTenantAzureAppConfiguration(
		this IServiceCollection services,
		Action<AzureMultiTenantOptions> action = null)
	{
		services.AddSingleton(serviceProvider =>
		{
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();

			var azureMultiTenantOptions = new AzureMultiTenantOptions
			{
				AzureAppConfigurationConnectionString = configuration["AzureAppConfigurationConnectionString"]
			};

			action?.Invoke(azureMultiTenantOptions);

			return azureMultiTenantOptions;
		});

		services.RemoveAll<IMultiTenantConfigurationSource>();

		services.AddSingleton<IMultiTenantConfigurationSource, AzureTenantSource>();

		return services;
	}
}