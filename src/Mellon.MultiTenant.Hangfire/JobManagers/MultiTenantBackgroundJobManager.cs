namespace Mellon.MultiTenant.Hangfire.JobManagers;

using System.Linq.Expressions;
using global::Hangfire;
using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Base.Interfaces;
using Mellon.MultiTenant.Hangfire.Interfaces;

internal class MultiTenantBackgroundJobManager(
	IMultiTenantConfiguration multiTenantConfiguration,
	IBackgroundJobClient backgroundJobClient,
	MultiTenantSettings multiTenantSettings) : IMultiTenantBackgroundJobManager
{
	public string Enqueue(Expression<Action> methodCall) => backgroundJobClient.Enqueue(multiTenantConfiguration.Tenant, methodCall);

	public string Enqueue(Expression<Func<Task>> methodCall) => backgroundJobClient.Enqueue(multiTenantConfiguration.Tenant, methodCall);

	public string Enqueue<T>(Expression<Action<T>> methodCall) => backgroundJobClient.Enqueue(multiTenantConfiguration.Tenant, methodCall);

	public string Enqueue<T>(Expression<Func<T, Task>> methodCall) => backgroundJobClient.Enqueue(multiTenantConfiguration.Tenant, methodCall);

	public IList<(string Tenant, string JobId)> EnqueueForAllTenants(Expression<Action> methodCall)
	{
		var data = new List<(string Tenant, string JobId)>();

		foreach (var tenant in multiTenantSettings.Tenants)
		{
			data.Add((tenant, backgroundJobClient.Enqueue(tenant, methodCall)));
		}

		return data;
	}

	public IList<(string Tenant, string JobId)> EnqueueForAllTenants(Expression<Func<Task>> methodCall)
	{
		var data = new List<(string Tenant, string JobId)>();

		foreach (var tenant in multiTenantSettings.Tenants)
		{
			data.Add((tenant, backgroundJobClient.Enqueue(tenant, methodCall)));
		}

		return data;
	}

	public IList<(string Tenant, string JobId)> EnqueueForAllTenants<T>(Expression<Action<T>> methodCall)
	{
		var data = new List<(string Tenant, string JobId)>();

		foreach (var tenant in multiTenantSettings.Tenants)
		{
			data.Add((tenant, backgroundJobClient.Enqueue(tenant, methodCall)));
		}

		return data;
	}

	public IList<(string Tenant, string JobId)> EnqueueForAllTenants<T>(Expression<Func<T, Task>> methodCall)
	{
		var data = new List<(string Tenant, string JobId)>();

		foreach (var tenant in multiTenantSettings.Tenants)
		{
			data.Add((tenant, backgroundJobClient.Enqueue(tenant, methodCall)));
		}

		return data;
	}

	public string Schedule(Expression<Action> methodCall, TimeSpan delay) => backgroundJobClient.Schedule(multiTenantConfiguration.Tenant, methodCall, delay);

	public string Schedule(Expression<Func<Task>> methodCall, TimeSpan delay) => backgroundJobClient.Schedule(multiTenantConfiguration.Tenant, methodCall, delay);

	public string Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay) => backgroundJobClient.Schedule(multiTenantConfiguration.Tenant, methodCall, delay);

	public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay) => backgroundJobClient.Schedule(multiTenantConfiguration.Tenant, methodCall, delay);

	public IList<(string Tenant, string JobId)> ScheduleForAllTenants(Expression<Action> methodCall, TimeSpan delay)
	{
		var data = new List<(string Tenant, string JobId)>();

		foreach (var tenant in multiTenantSettings.Tenants)
		{
			data.Add((tenant, backgroundJobClient.Schedule(tenant, methodCall, delay)));
		}

		return data;
	}

	public IList<(string Tenant, string JobId)> ScheduleForAllTenants(Expression<Func<Task>> methodCall, TimeSpan delay)
	{
		var data = new List<(string Tenant, string JobId)>();

		foreach (var tenant in multiTenantSettings.Tenants)
		{
			data.Add((tenant, backgroundJobClient.Schedule(tenant, methodCall, delay)));
		}

		return data;
	}

	public IList<(string Tenant, string JobId)> ScheduleForAllTenants<T>(Expression<Action<T>> methodCall, TimeSpan delay)
	{
		var data = new List<(string Tenant, string JobId)>();

		foreach (var tenant in multiTenantSettings.Tenants)
		{
			data.Add((tenant, backgroundJobClient.Schedule(tenant, methodCall, delay)));
		}

		return data;
	}

	public IList<(string Tenant, string JobId)> ScheduleForAllTenants<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
	{
		var data = new List<(string Tenant, string JobId)>();

		foreach (var tenant in multiTenantSettings.Tenants)
		{
			data.Add((tenant, backgroundJobClient.Schedule(tenant, methodCall, delay)));
		}

		return data;
	}
}
