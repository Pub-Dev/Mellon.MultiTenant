namespace Mellon.MultiTenant.Hangfire.Attributes;

using global::Hangfire;
using global::Hangfire.Client;
using global::Hangfire.Common;
using global::Hangfire.Server;
using Mellon.MultiTenant.Base;

public class PreventConcurrentExecutionJobFilterAttribute : JobFilterAttribute, IClientFilter, IServerFilter
{
	public void OnCreating(CreatingContext filterContext)
	{
		var processingJobs = JobStorage.Current.GetMonitoringApi().ProcessingJobs(0, 100);

		if (processingJobs.Any(x => string.Equals(GetResource(x.Value.Job), GetResource(filterContext.Job), StringComparison.InvariantCultureIgnoreCase)))
		{
			filterContext.SetJobParameter("Reason", "Job was already running");

			filterContext.Canceled = true;
		}
	}

	public void OnPerforming(PerformingContext filterContext)
	{
	}

	public void OnPerformed(PerformedContext filterContext)
	{
	}

	public void OnCreated(CreatedContext filterContext)
	{
	}

	private static string GetResource(Job job) => $"{job.Queue}-{job.Type.ToGenericTypeString()}-{job.Method}-{string.Join('-', job.Args)}";
}