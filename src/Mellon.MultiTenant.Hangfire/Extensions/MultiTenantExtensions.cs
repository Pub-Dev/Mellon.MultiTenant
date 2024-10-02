#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

using Hangfire;
using Mellon.MultiTenant.Hangfire.Filters;
using Mellon.MultiTenant.Hangfire.Interfaces;
using Mellon.MultiTenant.Hangfire.JobActivators;
using Mellon.MultiTenant.Hangfire.JobManagers;

public static class MultiTenantExtensions
{
    public static IServiceCollection AddMultiTenantHangfire(
        this IServiceCollection services)
    {
        services.AddScoped<IMultiTenantRecurringJobManager, MultiTenantRecurringJobManager>();

        services.AddScoped<IMultiTenantBackgroundJobManager, MultiTenantBackgroundJobManager>();

        services.AddSingleton<MultiTenantClientFilter>();

        return services;
    }

    public static IGlobalConfiguration UseMultiTenant(
        this IGlobalConfiguration globalConfiguration,
        IServiceProvider serviceProvider)
    {
        globalConfiguration.UseFilter(serviceProvider.GetRequiredService<MultiTenantClientFilter>());

        globalConfiguration.UseActivator(
            new MultiTenantHangfireJobActivator(serviceProvider.GetRequiredService<IServiceScopeFactory>()));

        return globalConfiguration;
    }
}
