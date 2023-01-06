using Hangfire;
using Hangfire.Console;
using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Extensions;
using Mellon.MultiTenant.Hangfire.Interfaces;
using Mellon.MultiTenant.Interfaces;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using WebApiHangfire.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMultiTenant()
    .AddMultiTenantHangfire();

builder.Services.AddHangfireServer((serviceProvider, config) =>
{
    var multiTenantSettings = serviceProvider.GetRequiredService<MultiTenantSettings>();
    var tenants = new List<string>(multiTenantSettings.Tenants);
    tenants.Add("cron");
    tenants.Add("default");

    config.ServerName = $"Worker-{Guid.NewGuid()}";
    config.Queues = tenants.ToArray();
    config.WorkerCount = 10;
});

builder.Services.AddHangfire((serviceProvider, config) =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangFire"));
    config.UseColouredConsoleLogProvider();
    config.UseSimpleAssemblyNameTypeSerializer();
    config.UseRecommendedSerializerSettings();
    config.UseConsole();
    config.UseMultiTenant(serviceProvider);
});

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console(theme: AnsiConsoleTheme.Code));

var app = builder.Build();

app.UseMultiTenant();

app.MapGet("add-email-sender", (
    IMultiTenantRecurringJobManager recurringJobManager,
    IMultiTenantBackgroundJobManager multiTenantBackgroundJobManager,
    IMultiTenantConfiguration multiTenantConfiguration) =>
{
    //recurringJobManager.AddOrUpdateForAllTenants<IEmailSender>("email-sender", job => job.ExecuteAsync(), Cron.Minutely());

    recurringJobManager.AddOrUpdateForAllTenants<IEmailSender>("long-email-sender", job => job.ExecuteLongJobAsync(1, null), "*/2 * * * *");

    //multiTenantBackgroundJobManager.Enqueue(() => Console.WriteLine("HELLO WORLD! FOR ALL TENANTS"));

    //multiTenantBackgroundJobManager.EnqueueForAllTenants(() => Console.WriteLine("HELLO WORLD! FOR ALL TENANTS"));

    //multiTenantBackgroundJobManager.Schedule(() => Console.WriteLine("HELLO WORLD! SINGLE 3 MINUTES"), TimeSpan.FromMinutes(3));

    //multiTenantBackgroundJobManager.ScheduleForAllTenants(() => Console.WriteLine("HELLO WORLD! SCHEDULERD 3 MINUTES"), TimeSpan.FromMinutes(3));

    return Results.Accepted();
});

app.UseHangfireDashboard();



app.Run();