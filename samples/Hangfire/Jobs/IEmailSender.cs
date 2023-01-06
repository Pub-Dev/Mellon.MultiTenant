using Hangfire;
using Hangfire.Server;

namespace WebApiHangfire.Jobs;

[Queue("cron")]
public interface IEmailSender
{
    Task ExecuteAsync();

    Task ExecuteLongJobAsync(int name, PerformContext context);
}

