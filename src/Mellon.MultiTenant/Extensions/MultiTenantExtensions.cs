using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Base.Interfaces;
using Mellon.MultiTenant.Interfaces;
using Mellon.MultiTenant.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mellon.MultiTenant.Extensions;

public static class MultiTenantExtensions
{
    private static IServiceCollection AddMultiTenant(
        this IServiceCollection services,
        MultiTenantOptions multiTenantOptions)
    {
        services.AddSingleton<MultiTenantSettings>((serviceProvider) =>
        {
            var multiTenantSettings = new MultiTenantSettings();

            var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var multiTenantSource = serviceProvider.GetRequiredService<ITenantConfigurationSource>();

            var multiTenantOptions = serviceProvider.GetRequiredService<MultiTenantOptions>();

            var tenants = MultiTenantSettings.LoadTenants(multiTenantOptions, configuration);

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

        services.AddSingleton<MultiTenantOptions>((serviceProvider) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            configuration.GetSection("MultiTenant").Bind(multiTenantOptions);

            return multiTenantOptions;
        });

        services.AddScoped<TenantSettings>();

        if (multiTenantOptions.CustomMultiTenantConfigurationSource is null)
        {
            services.AddSingleton<ITenantConfigurationSource, LocalTenantSource>();
        }
        else
        {
            services.AddSingleton(
                typeof(ITenantConfigurationSource),
                multiTenantOptions.CustomMultiTenantConfigurationSource);
        }

        services.AddScoped<IMultiTenantConfiguration, TenantConfiguration>();

        return services;
    }

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

    private static IEndpointRouteBuilder AddRefreshEndpoint(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGet("refresh-settings/{tenantName?}", RefreshEndpoint);
        
        routeBuilder.MapGet("refresh-settings", RefreshEndpoint);

        return routeBuilder;
    }

    private static IResult RefreshEndpoint(
                string tenantName,
                IConfiguration configuration,
                IHostEnvironment hostEnvironment,
                ITenantConfigurationSource multiTenantSource,
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
            foreach (var tenant in MultiTenantSettings.LoadTenants(multiTenantOptions, configuration))
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
