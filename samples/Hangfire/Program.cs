using Hangfire;
using Hangfire.Console;
using Hangfire.Tags;
using Hangfire.Tags.SqlServer;
using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Base.Interfaces;
using Mellon.MultiTenant.Extensions;
using Mellon.MultiTenant.Hangfire.Interfaces;
using WebApiHangfire.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddMultiTenant()
	.AddMultiTenantHangfire();

builder.Services.AddHangfireServer((serviceProvider, config) =>
{
	var multiTenantSettings = serviceProvider.GetRequiredService<MultiTenantSettings>();
	var tenants = new List<string>(multiTenantSettings.Tenants)
	{
		"cron",
		"default"
	};

	config.ServerName = $"Worker-{Guid.NewGuid()}";
	config.Queues = [.. tenants];
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

	var tagsOptions = new TagsOptions() { TagsListStyle = TagsListStyle.Dropdown };

	config.UseTagsWithSql(tagsOptions);
});

builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

app.UseMultiTenant();

app.MapGet("add-email-sender", (
	IMultiTenantRecurringJobManager recurringJobManager,
	IMultiTenantBackgroundJobManager multiTenantBackgroundJobManager,
	IMultiTenantConfiguration multiTenantConfiguration) =>
{
	// recurringJobManager.AddOrUpdateForAllTenants<IEmailSender>("email-sender", job => job.ExecuteAsync(), Cron.Minutely());
	// recurringJobManager.AddOrUpdateForAllTenants<IEmailSender>("long-email-sender", job => job.ExecuteLongJobAsync(1, null), "*/2 * * * *");
	multiTenantBackgroundJobManager.Enqueue(() => Console.WriteLine($"HELLO WORLD! for {multiTenantConfiguration.Tenant}"));

	recurringJobManager
		.AddOrUpdate<IEmailSender>("email-sender", job => job.ExecuteLongJobAsync(1, null), Cron.Minutely());

	// multiTenantBackgroundJobManager.EnqueueForAllTenants(() => Console.WriteLine("HELLO WORLD! FOR ALL TENANTS"));

	// multiTenantBackgroundJobManager.Schedule(() => Console.WriteLine("HELLO WORLD! SINGLE 3 MINUTES"), TimeSpan.FromMinutes(3));

	// multiTenantBackgroundJobManager.ScheduleForAllTenants(() => Console.WriteLine("HELLO WORLD! SCHEDULERD 3 MINUTES"), TimeSpan.FromMinutes(3));
	return Results.Accepted();
});

app.UseHangfireDashboard();

await app.RunAsync();