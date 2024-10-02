#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Mellon.MultiTenant;
using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Base.Interfaces;
using Mellon.MultiTenant.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class MultiTenantExtensions
{
	public static IServiceCollection AddMultiTenant(this IServiceCollection services, Action<MultiTenantOptions> options)
	{
		var multiTenantOptions = new MultiTenantOptions();

		options(multiTenantOptions);

		return services.AddMultiTenant(multiTenantOptions);
	}

	public static IServiceCollection AddMultiTenant(this IServiceCollection services)
	{
		var multiTenantOptions = new MultiTenantOptions();

		return services.AddMultiTenant(multiTenantOptions);
	}

	public static IApplicationBuilder UseMultiTenant(
		this IApplicationBuilder builder)
	{
		builder.UseMiddleware<HttpTenantIdentifierMiddleware>();

		if (builder is IEndpointRouteBuilder routeBuilder)
		{
			routeBuilder.AddRefreshEndpoint();
		}

		builder.ApplicationServices.GetRequiredService<MultiTenantSettings>();

		return builder;
	}

	private static IServiceCollection AddMultiTenant(
		this IServiceCollection services,
		MultiTenantOptions multiTenantOptions)
	{
		services.AddSingleton((serviceProvider) =>
		{
			var multiTenantSettings = new MultiTenantSettings();

			var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

			var configuration = serviceProvider.GetRequiredService<IConfiguration>();

			var multiTenantSource = serviceProvider.GetRequiredService<IMultiTenantConfigurationSource>();

			var multiTenantOptions = serviceProvider.GetRequiredService<MultiTenantOptions>();

			var tenants = MultiTenantSettings.LoadTenantsAsync(multiTenantOptions, configuration).GetAwaiter().GetResult();

			if (tenants is null || tenants.Length == 0)
			{
				throw new Exception("Invalid Configuration");
			}

			foreach (var tenant in tenants)
			{
				multiTenantSettings.LoadConfiguration(
					tenant,
					MultiTenantSettings.BuildTenantConfiguration(
						hostEnvironment,
						multiTenantSource,
						tenant));
			}

			return multiTenantSettings;
		});

		services.AddSingleton((serviceProvider) =>
		{
			var configuration = serviceProvider.GetRequiredService<IConfiguration>();

			configuration.GetSection("MultiTenant").Bind(multiTenantOptions);

			return multiTenantOptions;
		});

		services.AddScoped<TenantSettings>();

		if (multiTenantOptions.CustomMultiTenantConfigurationSource is null)
		{
			services.AddSingleton<IMultiTenantConfigurationSource, LocalMultiTenantSource>();
		}
		else
		{
			services.AddSingleton(
				typeof(IMultiTenantConfigurationSource),
				multiTenantOptions.CustomMultiTenantConfigurationSource);
		}

		services.AddScoped<IMultiTenantConfiguration, TenantConfiguration>();

		return services;
	}

	private static IEndpointRouteBuilder AddRefreshEndpoint(this IEndpointRouteBuilder routeBuilder)
	{
		routeBuilder.MapGet("refresh-settings/{tenantName?}", RefreshEndpointAsync);

		routeBuilder.MapGet("refresh-settings", RefreshEndpointAsync);

		return routeBuilder;
	}

	private static async Task<IResult> RefreshEndpointAsync(
				string tenantName,
				IConfiguration configuration,
				IHostEnvironment hostEnvironment,
				IMultiTenantConfigurationSource multiTenantSource,
				MultiTenantOptions multiTenantOptions,
				MultiTenantSettings multiTenantSettings)
	{
		bool TryFindAndRefreshSettings(string tenantName)
		{
			if (multiTenantSettings.GetConfigurations.TryGetValue(tenantName, out var conf))
			{
				if (conf is IConfigurationRoot configurationRoot)
				{
					configurationRoot.Reload();
				}

				return true;
			}

			return false;
		}

		if (!string.IsNullOrEmpty(tenantName))
		{
			if (!TryFindAndRefreshSettings(tenantName))
			{
				return Results.NotFound(new { Message = "Tenant not found" });
			}
		}
		else
		{
			var tenants = await MultiTenantSettings.LoadTenantsAsync(multiTenantOptions, configuration);

			foreach (var tenant in tenants)
			{
				if (!TryFindAndRefreshSettings(tenant))
				{
					multiTenantSettings.LoadConfiguration(
						tenant,
						MultiTenantSettings.BuildTenantConfiguration(
							hostEnvironment,
							multiTenantSource,
							tenant));
				}
			}
		}

		return Results.Ok(new { Message = "Refresh Done!" });
	}
}