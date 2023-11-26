using Hangfire.Console;
using Hangfire.Server;
using Mellon.MultiTenant.Interfaces;

namespace WebApiHangfire.Jobs;

public class EmailSender(ILogger<EmailSender> logger,
        IMultiTenantConfiguration multiTenantConfiguration) : IEmailSender
{
   
    public async Task ExecuteAsync()
    {
        logger.LogInformation($"Processing e-mail sending for {multiTenantConfiguration.Tenant}");

        await Task.FromResult(true);
    }

    public async Task ExecuteLongJobAsync(int name, PerformContext context)
    {
        var items = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

        var bar = context.WriteProgressBar();

        foreach (var item in items.WithProgress(bar))
        {
            context.WriteLine(ConsoleTextColor.Gray, $"Starting the process for the object {item}");

            await Task.Delay(2000);

            context.WriteLine(ConsoleTextColor.Blue, $"process for the object {item} completed!");
        }
    }
}
