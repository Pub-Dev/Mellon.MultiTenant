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

        if (timeZone == null)
        {
            timeZone = TimeZoneInfo.Utc;
        }

        Job job = Job.FromExpression(methodCall, queue);

        manager.AddOrUpdate(recurringJobId, job, cronExpression, new RecurringJobOptions
        {
            TimeZone = timeZone
        });
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

        Job job = Job.FromExpression(methodCall, queue);

        manager.AddOrUpdateForAllTenants(recurringJobId, job, cronExpression, timeZone ?? TimeZoneInfo.Utc);
    }

    public static void AddOrUpdateForAllTenants(
        this IMultiTenantRecurringJobManager manager,
        string recurringJobId,
        Job job,
        string cronExpression,
        TimeZoneInfo timeZone)
    {
        if (manager == null)
        {
            throw new ArgumentNullException("manager");
        }

        if (timeZone == null)
        {
            throw new ArgumentNullException("timeZone");
        }

        manager.AddOrUpdateForAllTenants(recurringJobId, job, cronExpression, new RecurringJobOptions
        {
            TimeZone = timeZone
        });
    }
}
