namespace Mellon.MultiTenant.Hangfire.Interfaces;

using global::Hangfire;
using global::Hangfire.Common;

public interface IMultiTenantRecurringJobManager : IRecurringJobManager
{
	public void AddOrUpdateForAllTenants(string recurringJobId, Job job, string cronExpression, RecurringJobOptions options);

	public void RemoveIfExistsForAllTenants(string recurringJobId);

	public void TriggerForAllTenants(string recurringJobId);
}