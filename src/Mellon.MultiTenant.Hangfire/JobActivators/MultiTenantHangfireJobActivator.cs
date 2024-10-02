namespace Mellon.MultiTenant.Hangfire.JobActivators;

using global::Hangfire;
using Mellon.MultiTenant.Base;
using Microsoft.Extensions.DependencyInjection;
using HangfireAspNetCore = global::Hangfire.AspNetCore;

public class MultiTenantHangfireJobActivator(
	IServiceScopeFactory serviceScopeFactory)
		: HangfireAspNetCore.AspNetCoreJobActivator(serviceScopeFactory)
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