using Hangfire;
using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Hangfire.Interfaces;
using Mellon.MultiTenant.Interfaces;
using System.Linq.Expressions;

namespace Mellon.MultiTenant.Hangfire.JobManagers;

internal class MultiTenantBackgroundJobManager(
    IMultiTenantConfiguration multiTenantConfiguration,
    MultiTenantSettings multiTenantSettings) : IMultiTenantBackgroundJobManager
{
    public string Enqueue(Expression<Action> methodCall)
    {
        return BackgroundJob.Enqueue(multiTenantConfiguration.Tenant, methodCall);
    }

    public string Enqueue(Expression<Func<Task>> methodCall)
    {
        return BackgroundJob.Enqueue(multiTenantConfiguration.Tenant, methodCall);
    }

    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        return BackgroundJob.Enqueue(multiTenantConfiguration.Tenant, methodCall);
    }

    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        return BackgroundJob.Enqueue(multiTenantConfiguration.Tenant, methodCall);
    }

    public IList<(string tenant, string jobId)> EnqueueForAllTenants(Expression<Action> methodCall)
    {
        var data =  new List<(string tenant, string jobId)>();

        foreach (var tenant in multiTenantSettings.Tenants)
        {
            data.Add((tenant, BackgroundJob.Enqueue(tenant, methodCall)));
        }

        return data;
    }

    public IList<(string tenant, string jobId)> EnqueueForAllTenants(Expression<Func<Task>> methodCall)
    {
        var data = new List<(string tenant, string jobId)>();

        foreach (var tenant in multiTenantSettings.Tenants)
        {
            data.Add((tenant, BackgroundJob.Enqueue(tenant, methodCall)));
        }

        return data;
    }

    public IList<(string tenant, string jobId)> EnqueueForAllTenants<T>(Expression<Action<T>> methodCall)
    {
        var data = new List<(string tenant, string jobId)>();

        foreach (var tenant in multiTenantSettings.Tenants)
        {
            data.Add((tenant, BackgroundJob.Enqueue(tenant, methodCall)));
        }

        return data;
    }

    public IList<(string tenant, string jobId)> EnqueueForAllTenants<T>(Expression<Func<T, Task>> methodCall)
    {
        var data = new List<(string tenant, string jobId)>();

        foreach (var tenant in multiTenantSettings.Tenants)
        {
            data.Add((tenant, BackgroundJob.Enqueue(tenant, methodCall)));
        }

        return data;
    }

    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(multiTenantConfiguration.Tenant, methodCall, delay);
    }

    public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(multiTenantConfiguration.Tenant, methodCall, delay);
    }

    public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(multiTenantConfiguration.Tenant, methodCall, delay);
    }

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(multiTenantConfiguration.Tenant, methodCall, delay);
    }

    public IList<(string tenant, string jobId)> ScheduleForAllTenants(Expression<Action> methodCall, TimeSpan delay)
    {
        var data = new List<(string tenant, string jobId)>();

        foreach (var tenant in multiTenantSettings.Tenants)
        {
            data.Add((tenant, BackgroundJob.Schedule(tenant, methodCall, delay)));
        }

        return data;
    }

    public IList<(string tenant, string jobId)> ScheduleForAllTenants(Expression<Func<Task>> methodCall, TimeSpan delay)
    {
        var data = new List<(string tenant, string jobId)>();

        foreach (var tenant in multiTenantSettings.Tenants)
        {
            data.Add((tenant, BackgroundJob.Schedule(tenant, methodCall, delay)));
        }

        return data;
    }

    public IList<(string tenant, string jobId)> ScheduleForAllTenants<T>(Expression<Action<T>> methodCall, TimeSpan delay)
    {
        var data = new List<(string tenant, string jobId)>();

        foreach (var tenant in multiTenantSettings.Tenants)
        {
            data.Add((tenant, BackgroundJob.Schedule(tenant, methodCall, delay)));
        }

        return data;
    }

    public IList<(string tenant, string jobId)> ScheduleForAllTenants<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        var data = new List<(string tenant, string jobId)>();

        foreach (var tenant in multiTenantSettings.Tenants)
        {
            data.Add((tenant, BackgroundJob.Schedule(tenant, methodCall, delay)));
        }

        return data;
    }
}
