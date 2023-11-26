using Hangfire;
using Hangfire.Common;
using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Hangfire.Interfaces;
using Mellon.MultiTenant.Interfaces;

namespace Mellon.MultiTenant.Hangfire.JobManagers;

internal class MultiTenantRecurringJobManager(
    IRecurringJobManager recurringJobManager,
    IMultiTenantConfiguration multiTenantConfiguration,
    MultiTenantSettings multiTenantSettings) : IMultiTenantRecurringJobManager
{
    public void AddOrUpdate(string recurringJobId, Job job, string cronExpression, RecurringJobOptions options)
    {
        var jobName = GetTenantJobName(multiTenantConfiguration.Tenant, recurringJobId);

        recurringJobManager.AddOrUpdate(jobName, job, cronExpression, options);
    }

    public void AddOrUpdateForAllTenants(string recurringJobId, Job job, string cronExpression, RecurringJobOptions options)
    {
        foreach (var tenant in multiTenantSettings.Tenants)
        {
            var jobName = GetTenantJobName(tenant, recurringJobId);

            recurringJobManager.AddOrUpdate(jobName, job, cronExpression, options);
        }
    }

    public void RemoveIfExists(string recurringJobId)
    {
        var jobName = GetTenantJobName(multiTenantConfiguration.Tenant, recurringJobId);

        recurringJobManager.RemoveIfExists(jobName);
    }

    public void RemoveIfExistsForAllTenants(string recurringJobId)
    {
        foreach (var tenant in multiTenantSettings.Tenants)
        {
            var jobName = GetTenantJobName(tenant, recurringJobId);

            recurringJobManager.RemoveIfExists(jobName);
        }
    }

    public void Trigger(string recurringJobId)
    {
        var jobName = GetTenantJobName(multiTenantConfiguration.Tenant, recurringJobId);

        recurringJobManager.Trigger(jobName);
    }

    public void TriggerForAllTenants(string recurringJobId)
    {
        foreach (var tenant in multiTenantSettings.Tenants)
        {
            var jobName = GetTenantJobName(tenant, recurringJobId);

            recurringJobManager.Trigger(jobName);
        }
    }

    private static string GetTenantJobName(string tenant, string recurringJobId)
    {
        if (recurringJobId.Contains($"{tenant}@"))
        {
            return recurringJobId;
        }

        return $"{tenant}@{recurringJobId}";
    }
}