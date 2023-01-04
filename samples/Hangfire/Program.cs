using Hangfire;
using Hangfire.Console;
using Mellon.MultiTenant.Extensions;
using Mellon.MultiTenant.Hangfire.Interfaces;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using WebApiHangfire.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMultiTenant()
    .AddMultiTenantHangfire();

builder.Services.AddHangfireServer(config =>
{
    config.ServerName = $"Worker-{Guid.NewGuid()}";
    config.Queues = new string[] { "cron" };
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
    IMultiTenantRecurringJobManager jobManager) =>
{
    jobManager.AddOrUpdateForAllTenants<IEmailSender>("email-sender", job => job.ExecuteAsync(), Cron.Minutely());

    return Results.Accepted();
});

app.UseHangfireDashboard();

app.Run();