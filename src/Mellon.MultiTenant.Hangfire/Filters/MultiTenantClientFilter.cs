using Hangfire.Client;
using Hangfire.Console;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mellon.MultiTenant.Hangfire.Filters;

internal class MultiTenantClientFilter : IClientFilter, IServerFilter
{
    private readonly ILogger<MultiTenantClientFilter> _logger;
    private readonly IServiceScopeFactory _factory;

    public MultiTenantClientFilter(
        ILogger<MultiTenantClientFilter> logger,
        IServiceScopeFactory factory
        )
    {
        _logger = logger;
        _factory = factory;
    }

    public void OnCreating(CreatingContext filterContext)
    {
        var tenantName = ExtractTenantFromContext(filterContext);

        filterContext.SetJobParameter("TenantName", tenantName);
    }

    private static string ExtractTenantFromContext(CreatingContext filterContext)
    {
        var tenantName = default(string);

        if (filterContext.Parameters.TryGetValue("RecurringJobId", out object jobId) && jobId.ToString().Contains("@"))
        {
            tenantName = jobId.ToString().Split("@").First();
        }

        if (string.IsNullOrEmpty(tenantName) && filterContext.InitialState is EnqueuedState enqueuedState)
        {
            if (enqueuedState.Queue == EnqueuedState.DefaultQueue)
            {
                tenantName = filterContext.Job.Queue;
            }
            else
            {
                tenantName = enqueuedState.Queue;
            }
        }

        if (string.IsNullOrEmpty(tenantName) && filterContext.InitialState is ScheduledState scheduledState)
        {
            tenantName = filterContext.Job.Queue;
        }

        return tenantName;
    }

    public void OnCreated(CreatedContext filterContext)
    {
        filterContext.Parameters.TryGetValue("RecurringJobId", out var jobId);

        filterContext.Parameters.TryGetValue("TenantName", out var tenantName);

        _logger.LogInformation(
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
}
