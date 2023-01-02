using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Extensions;
using Mellon.MultiTenant.Interfaces;
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

app.MapGet("/", (IMultiTenantConfiguration configuration) =>
{
    return new
    {
        Tenant = configuration.Tenant,
        Message = configuration["Message"],
    };
});

app.MapGet("/products", async (DataBaseContext dataBaseContext) =>
{
    return await dataBaseContext.Products.ToListAsync();
});

var tenants = app.Services.GetRequiredService<MultiTenantSettings>();

foreach (var tenant in tenants.Tenants)
{
    using (var scope = app.Services.CreateScope())
    {
        var tenantSettings = scope.ServiceProvider.GetRequiredService<TenantSettings>();

        tenantSettings.SetCurrentTenant(tenant);

        var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

        await db.Database.MigrateAsync();
    }
}

app.Run();
