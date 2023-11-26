using System.Linq.Expressions;

namespace Mellon.MultiTenant.Hangfire.Interfaces;

public interface IMultiTenantBackgroundJobManager
{
    IList<(string tenant, string jobId)> EnqueueForAllTenants(Expression<Action> methodCall);
    IList<(string tenant, string jobId)> EnqueueForAllTenants(Expression<Func<Task>> methodCall);
    IList<(string tenant, string jobId)> EnqueueForAllTenants<T>(Expression<Action<T>> methodCall);
    IList<(string tenant, string jobId)> EnqueueForAllTenants<T>(Expression<Func<T, Task>> methodCall);
    string Enqueue(Expression<Action> methodCall);
    string Enqueue(Expression<Func<Task>> methodCall);
    string Enqueue<T>(Expression<Action<T>> methodCall);
    string Enqueue<T>(Expression<Func<T, Task>> methodCall);
    IList<(string tenant, string jobId)> ScheduleForAllTenants(Expression<Action> methodCall, TimeSpan delay);
    IList<(string tenant, string jobId)> ScheduleForAllTenants(Expression<Func<Task>> methodCall, TimeSpan delay);
    IList<(string tenant, string jobId)> ScheduleForAllTenants<T>(Expression<Action<T>> methodCall, TimeSpan delay);
    IList<(string tenant, string jobId)> ScheduleForAllTenants<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);
    string Schedule(Expression<Action> methodCall, TimeSpan delay);
    string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay);
    string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);
    string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);
}
