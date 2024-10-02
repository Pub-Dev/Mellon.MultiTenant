namespace WebApiHangfire.Jobs;

using Hangfire.Console;
using Hangfire.Server;
using Hangfire.Tags;
using Mellon.MultiTenant.Base.Interfaces;

public class EmailSender(ILogger<EmailSender> logger,
		IMultiTenantConfiguration multiTenantConfiguration) : IEmailSender
{
	public Task ExecuteAsync(PerformContext context)
	{
		context.AddTags(multiTenantConfiguration.Tenant);

		logger.LogInformation($"Processing e-mail sending for {multiTenantConfiguration.Tenant}");

		return Task.FromResult(true);
	}

	public async Task ExecuteLongJobAsync(int name, PerformContext context)
	{
		context.AddTags(multiTenantConfiguration.Tenant);

		var items = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

		var bar = context.WriteProgressBar();

		foreach (var item in items.WithProgress(bar))
		{
			context.WriteLine(ConsoleTextColor.Gray, $"Starting the process for the object {item}");

			await Task.Delay(2000, context.CancellationToken.ShutdownToken);

			context.WriteLine(ConsoleTextColor.Blue, $"process for the object {item} completed!");
		}
	}
}