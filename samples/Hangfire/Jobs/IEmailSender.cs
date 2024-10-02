namespace WebApiHangfire.Jobs;

using Hangfire;
using Hangfire.Server;

[Queue("tenant-name")]
public interface IEmailSender
{
	Task ExecuteAsync(PerformContext context);

	Task ExecuteLongJobAsync(int name, PerformContext context);
}