using Mellon.MultiTenant;
using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Extensions;
using Mellon.MultiTenant.Interfaces;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddMultiTenant(x => x.WithCustonTenantConfigurationSource<LocalTenantSource>());

builder.Services.AddDbContext<DataBaseContext>(
    (serviceProvider, options) =>
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
