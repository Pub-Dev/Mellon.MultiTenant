using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.EntityFrameworkCore;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMultiTenant();

builder.Services.AddDbContext<DataBaseContext>(
	(IServiceProvider serviceProvider, DbContextOptionsBuilder options) =>
	{
		var configuration = serviceProvider.GetRequiredService<IMultiTenantConfiguration>();

		options.UseSqlServer(configuration?["ConnectionStrings:DefaultConnection"]);
	});

var app = builder.Build();

app.UseMultiTenant();

app.MapGet("/", (IMultiTenantConfiguration configuration) => new
{
	configuration.Tenant,
	Message = configuration["Message"],
});

app.MapGet("/products", async (DataBaseContext dataBaseContext) => await dataBaseContext.Products.ToListAsync(cancellationToken: app.Lifetime.ApplicationStopped));

var tenants = app.Services.GetRequiredService<MultiTenantSettings>();

foreach (var tenant in tenants.Tenants)
{
	using var scope = app.Services.CreateScope();

	var tenantSettings = scope.ServiceProvider.GetRequiredService<TenantSettings>();

	tenantSettings.SetCurrentTenant(tenant);

	var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

	await db.Database.MigrateAsync(cancellationToken: app.Lifetime.ApplicationStopped);
}

await app.RunAsync();