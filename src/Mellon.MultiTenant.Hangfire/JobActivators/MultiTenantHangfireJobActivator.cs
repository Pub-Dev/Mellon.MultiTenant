using Hangfire;
using Mellon.MultiTenant.Base;
using Microsoft.Extensions.DependencyInjection;

public class MultiTenantHangfireJobActivator : Hangfire.AspNetCore.AspNetCoreJobActivator
{
    public MultiTenantHangfireJobActivator(IServiceScopeFactory serviceScopeFactory) :
        base(serviceScopeFactory)
    {
    }

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