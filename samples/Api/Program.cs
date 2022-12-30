using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Extensions;
using Mellon.MultiTenant.Interfaces;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services
    .AddMultiTenant();

var app = builder.Build();

app.UseMultiTenant();

app.MapGet("/full", (IMultiTenantConfiguration configuration) =>
{
    return (((Mellon.MultiTenant.Base.TenantConfiguration)configuration).Configuration as IConfigurationRoot).GetDebugView();
});

app.MapGet("/", (
    IMultiTenantConfiguration configuration,
    MultiTenantOptions multiTenantOptions) =>
{
    return new
    {
        multiTenantOptions,
        Tenant = configuration.Tenant,
        Message = configuration["Message"],
    };
});

app.Run();
