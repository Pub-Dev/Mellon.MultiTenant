using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Mellon.MultiTenant.Base;

namespace Mellon.MultiTenant.Hangfire.Attributes;

public class PreventConcurrentExecutionJobFilter : JobFilterAttribute, IClientFilter, IServerFilter
{
    public void OnCreating(CreatingContext filterContext)
    {
        var processingJobs = JobStorage.Current.GetMonitoringApi().ProcessingJobs(0, 100);

        if (processingJobs.Any(x => GetResource(x.Value.Job) == GetResource(filterContext.Job)))
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

    private static string GetResource(Job job)
    {
        var resourceName = $"{job.Queue}-{job.Type.ToGenericTypeString()}-{job.Method}-{string.Join('-', job.Args)}".ToLowerInvariant();

        return resourceName;
    }
}
