using Hangfire;
using Hangfire.Common;
using Mellon.MultiTenant.Hangfire.Interfaces;
using System.Linq.Expressions;

namespace Mellon.MultiTenant.Extensions;

public static class MultiTenantRecurringJobManagerExtensions
{
    public static void AddOrUpdate<T>(
        this IMultiTenantRecurringJobManager manager, 
        string recurringJobId, 
        Expression<Func<T, Task>> methodCall, 
        string cronExpression, 
        TimeZoneInfo timeZone = null, 
        string queue = "default")
    {
        if (manager == null)
        {
            throw new ArgumentNullException("manager");
        }

        if (recurringJobId == null)
        {
            throw new ArgumentNullException("recurringJobId");
        }

        if (methodCall == null)
        {
            throw new ArgumentNullException("methodCall");
        }

        if (cronExpression == null)
        {
            throw new ArgumentNullException("cronExpression");
        }

        Job job = Job.FromExpression(methodCall);

        manager.AddOrUpdate(recurringJobId, job, cronExpression, timeZone ?? TimeZoneInfo.Utc, queue);
    }

    public static void AddOrUpdateForAllTenants<T>(
        this IMultiTenantRecurringJobManager manager,
        string recurringJobId,
        Expression<Func<T, Task>> methodCall,
        string cronExpression,
        TimeZoneInfo timeZone = null,
        string queue = "default")
    {
        if (manager == null)
        {
            throw new ArgumentNullException("manager");
        }

        if (recurringJobId == null)
        {
            throw new ArgumentNullException("recurringJobId");
        }

        if (methodCall == null)
        {
            throw new ArgumentNullException("methodCall");
        }

        if (cronExpression == null)
        {
            throw new ArgumentNullException("cronExpression");
        }

        Job job = Job.FromExpression(methodCall);

        manager.AddOrUpdateForAllTenants(recurringJobId, job, cronExpression, timeZone ?? TimeZoneInfo.Utc, queue);
    }

    public static void AddOrUpdateForAllTenants(
        this IMultiTenantRecurringJobManager manager,
        string recurringJobId, 
        Job job, 
        string cronExpression, 
        TimeZoneInfo timeZone, 
        string queue)
    {
        if (manager == null)
        {
            throw new ArgumentNullException("manager");
        }

        if (timeZone == null)
        {
            throw new ArgumentNullException("timeZone");
        }

        if (queue == null)
        {
            throw new ArgumentNullException("queue");
        }

        manager.AddOrUpdateForAllTenants(recurringJobId, job, cronExpression, new RecurringJobOptions
        {
            QueueName = queue,
            TimeZone = timeZone
        });
    }
}
