using Mellon.MultiTenant.Interfaces;
using Mellon.MultiTenant.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mellon.MultiTenant.Extensions;

public static class MultiTenantExtensions
{
    private static IServiceCollection AddMultiTenant(
        this IServiceCollection services, MultiTenantOptions multiTenantOptions)
    {
        services.AddSingleton((serviceProvider) =>
        {
            var multiTenantSettings = new MultiTenantSettings();

            var hostEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var multiTenantOptions = serviceProvider.GetRequiredService<MultiTenantOptions>();

            var tenants = multiTenantSettings.LoadTenants(multiTenantOptions, configuration);

            if (tenants is null || tenants.Length == 0)
            {
                throw new Exception("Invalid Configuration");
            }

            foreach (var tenant in tenants)
            {
                multiTenantSettings.LoadConfiguration(
                    tenant,
                    multiTenantSettings.BuildTenantConfiguration(multiTenantOptions, hostEnvironment, tenant));
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
            routeBuilder.Map("refresh-settings", (
                MultiTenantOptions multiTenantOptions,
                IConfiguration configuration,
                IHostEnvironment hostEnvironment,
                MultiTenantSettings multiTenantSettings) =>
            {
                foreach (var tenant in multiTenantSettings.LoadTenants(multiTenantOptions, configuration))
                {
                    if (multiTenantSettings.GetConfigurations.TryGetValue(tenant, out var conf))
                    {
                        if (conf is IConfigurationRoot root)
                        {
                            root.Reload();
                        }
                    }
                    else
                    {
                        multiTenantSettings.LoadConfiguration(
                            tenant,
                            multiTenantSettings.BuildTenantConfiguration(multiTenantOptions, hostEnvironment, tenant));
                    }
                }

                return new { Message = "Refresh Done!" };
            });
        }

        builder.ApplicationServices.GetRequiredService<MultiTenantSettings>();

        return builder;
    }
}
