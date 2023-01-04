using Hangfire.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mellon.MultiTenant.Hangfire.Filters;

internal class MultiTenantClientFilter : IClientFilter
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
        if (filterContext.Parameters.TryGetValue("RecurringJobId", out object jobId) && jobId.ToString().Contains("@"))
        {
            var tenantName = jobId.ToString().Split("@").First();

            filterContext.SetJobParameter("TenantName", tenantName);
        }
    }

    public void OnCreated(CreatedContext filterContext)
    {
        _logger.LogWarning(
            "Job `{recurringJobId}` that is based on method `{Name}` has been created with id `{Id}` for Tenant `{tenant}`",
            filterContext.Parameters["RecurringJobId"],
            filterContext.Job.Method.Name,
            filterContext.BackgroundJob?.Id,
            filterContext.Parameters["TenantName"]);
    }
}
