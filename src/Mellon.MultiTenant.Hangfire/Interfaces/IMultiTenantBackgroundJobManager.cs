namespace Mellon.MultiTenant.Hangfire.Interfaces;

using System.Linq.Expressions;

public interface IMultiTenantBackgroundJobManager
{
	IList<(string Tenant, string JobId)> EnqueueForAllTenants(Expression<Action> methodCall);

	IList<(string Tenant, string JobId)> EnqueueForAllTenants(Expression<Func<Task>> methodCall);

	IList<(string Tenant, string JobId)> EnqueueForAllTenants<T>(Expression<Action<T>> methodCall);

	IList<(string Tenant, string JobId)> EnqueueForAllTenants<T>(Expression<Func<T, Task>> methodCall);

	string Enqueue(Expression<Action> methodCall);

	string Enqueue(Expression<Func<Task>> methodCall);

	string Enqueue<T>(Expression<Action<T>> methodCall);

	string Enqueue<T>(Expression<Func<T, Task>> methodCall);

	IList<(string Tenant, string JobId)> ScheduleForAllTenants(Expression<Action> methodCall, TimeSpan delay);

	IList<(string Tenant, string JobId)> ScheduleForAllTenants(Expression<Func<Task>> methodCall, TimeSpan delay);

	IList<(string Tenant, string JobId)> ScheduleForAllTenants<T>(Expression<Action<T>> methodCall, TimeSpan delay);

	IList<(string Tenant, string JobId)> ScheduleForAllTenants<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);

	string Schedule(Expression<Action> methodCall, TimeSpan delay);

	string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay);

	string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);

	string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);
}