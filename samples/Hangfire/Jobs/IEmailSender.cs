using Hangfire;

namespace WebApiHangfire.Jobs;

[Queue("cron")]
public interface IEmailSender
{
    Task ExecuteAsync();
}

