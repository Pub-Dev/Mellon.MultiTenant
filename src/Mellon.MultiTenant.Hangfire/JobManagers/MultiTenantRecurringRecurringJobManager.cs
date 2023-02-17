using Hangfire;
using Hangfire.Common;
using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Hangfire.Interfaces;
using Mellon.MultiTenant.Interfaces;

namespace Mellon.MultiTenant.Hangfire.JobManagers;

internal class MultiTenantRecurringJobManager : IMultiTenantRecurringJobManager
{
    private readonly IRecurringJobManager _recurringJobManager;

    private readonly IMultiTenantConfiguration _multiTenantConfiguration;

    private readonly MultiTenantSettings _multiTenantSettings;

    public MultiTenantRecurringJobManager(
        IRecurringJobManager recurringJobManager,
        IMultiTenantConfiguration multiTenantConfiguration,
        MultiTenantSettings multiTenantSettings)
    {
        _recurringJobManager = recurringJobManager;

        _multiTenantConfiguration = multiTenantConfiguration;

        _multiTenantSettings = multiTenantSettings;
    }

    public void AddOrUpdate(string recurringJobId, Job job, string cronExpression, RecurringJobOptions options)
    {
        var jobName = GetTenantJobName(_multiTenantConfiguration.Tenant, recurringJobId);

        _recurringJobManager.AddOrUpdate(jobName, job, cronExpression, options);
    }

    public void AddOrUpdateForAllTenants(string recurringJobId, Job job, string cronExpression, RecurringJobOptions options)
    {
        foreach (var tenant in _multiTenantSettings.Tenants)
        {
            var jobName = GetTenantJobName(tenant, recurringJobId);

            _recurringJobManager.AddOrUpdate(jobName, job, cronExpression, options);
        }
    }

    public void RemoveIfExists(string recurringJobId)
    {
        var jobName = GetTenantJobName(_multiTenantConfiguration.Tenant, recurringJobId);

        _recurringJobManager.RemoveIfExists(jobName);
    }

    public void RemoveIfExistsForAllTenants(string recurringJobId)
    {
        foreach (var tenant in _multiTenantSettings.Tenants)
        {
            var jobName = GetTenantJobName(tenant, recurringJobId);

            _recurringJobManager.RemoveIfExists(jobName);
        }
    }

    public void Trigger(string recurringJobId)
    {
        var jobName = GetTenantJobName(_multiTenantConfiguration.Tenant, recurringJobId);

        _recurringJobManager.Trigger(recurringJobId);
    }

    public void TriggerForAllTenants(string recurringJobId)
    {
        foreach (var tenant in _multiTenantSettings.Tenants)
        {
            var jobName = GetTenantJobName(tenant, recurringJobId);

            _recurringJobManager.Trigger(jobName);
        }
    }

    private string GetTenantJobName(string tenant, string recurringJobId)
    {
        if (recurringJobId.Contains($"{tenant}@"))
        {
            return recurringJobId;
        }

        return $"{tenant}@{recurringJobId}";
    }
}