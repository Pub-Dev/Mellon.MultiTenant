namespace Mellon.MultiTenant.Hangfire.Filters;

using global::Hangfire.Client;
using global::Hangfire.Console;
using global::Hangfire.Server;
using global::Hangfire.States;
using global::Hangfire.Storage;
using Microsoft.Extensions.Logging;

internal class MultiTenantClientFilter(ILogger<MultiTenantClientFilter> logger) : IClientFilter, IServerFilter, IApplyStateFilter
{
	public void OnCreating(CreatingContext filterContext)
	{
		var tenantName = ExtractTenantFromContext(filterContext);

		filterContext.SetJobParameter("TenantName", tenantName);
	}

	public void OnCreated(CreatedContext filterContext)
	{
		filterContext.Parameters.TryGetValue("RecurringJobId", out var jobId);

		filterContext.Parameters.TryGetValue("TenantName", out var tenantName);

		logger.LogInformation(
			"Job `{recurringJobId}` that is based on method `{Name}` has been created with id `{Id}` for Tenant `{tenant}`",
			jobId,
			filterContext.Job.Method.Name,
			filterContext.BackgroundJob?.Id,
			tenantName);
	}

	public void OnPerforming(PerformingContext filterContext)
	{
		var tenantName = filterContext.GetJobParameter<string>("TenantName");

		filterContext.WriteLine(ConsoleTextColor.Yellow, $"Starting job for the tenant {tenantName} 🚀");
	}

	public void OnPerformed(PerformedContext filterContext)
	{
		var tenantName = filterContext.GetJobParameter<string>("TenantName");

		filterContext.WriteLine(ConsoleTextColor.Yellow, $"Process completed for the {tenantName} 🚀");
	}

	public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
	{
		var queue = context.GetJobParameter<string>("TenantName");

		if (!string.IsNullOrWhiteSpace(queue))
		{
			if (context.NewState is EnqueuedState newState)
			{
				if (string.Equals(newState.Queue, "tenant-name", StringComparison.InvariantCultureIgnoreCase))
				{
					newState.Queue = queue;
				}
			}
		}
	}

	public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
	{
	}

	private static string ExtractTenantFromContext(CreatingContext filterContext)
	{
		var tenantName = default(string);

		if (filterContext.Parameters.TryGetValue("RecurringJobId", out var jobId) && jobId.ToString().Contains('@', StringComparison.InvariantCultureIgnoreCase))
		{
			tenantName = jobId.ToString().Split("@")[0];
		}

		if (string.IsNullOrEmpty(tenantName) && filterContext.InitialState is EnqueuedState enqueuedState)
		{
			if (string.Equals(enqueuedState.Queue, EnqueuedState.DefaultQueue, StringComparison.InvariantCultureIgnoreCase))
			{
				tenantName = filterContext.Job.Queue;
			}
			else
			{
				tenantName = enqueuedState.Queue;
			}
		}

		if (string.IsNullOrEmpty(tenantName) && filterContext.InitialState is ScheduledState)
		{
			tenantName = filterContext.Job.Queue;
		}

		return tenantName;
	}
}