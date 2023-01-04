using Hangfire;
using Hangfire.Common;

namespace Mellon.MultiTenant.Hangfire.Interfaces;

public interface IMultiTenantRecurringJobManager : IRecurringJobManager
{
    public void AddOrUpdateForAllTenants(string recurringJobId, Job job, string cronExpression, RecurringJobOptions options);
    public void RemoveIfExistsForAllTenants(string recurringJobId);
    public void TriggerForAllTenants(string recurringJobId);
}