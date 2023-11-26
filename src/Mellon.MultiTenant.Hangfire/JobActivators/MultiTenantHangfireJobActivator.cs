using Hangfire;
using Mellon.MultiTenant.Base;
using Microsoft.Extensions.DependencyInjection;
using HangfireAspNetCore = Hangfire.AspNetCore;

namespace Mellon.MultiTenant.Hangfire.JobActivators;

public class MultiTenantHangfireJobActivator(IServiceScopeFactory serviceScopeFactory) : HangfireAspNetCore.AspNetCoreJobActivator(serviceScopeFactory)
{
    public override JobActivatorScope BeginScope(JobActivatorContext context)
    {
        var scope = base.BeginScope(context);

        var tenantName = context.GetJobParameter<string>("TenantName");

        if (!string.IsNullOrEmpty(tenantName))
        {
            var tenantSettings = (TenantSettings)scope.Resolve(typeof(TenantSettings));

            tenantSettings.SetCurrentTenant(tenantName);
        }

        return scope;
    }
}