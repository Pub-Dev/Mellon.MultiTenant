using Mellon.MultiTenant.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Mellon.MultiTenant.Middlewares;

public class HttpTenantIdentifierMiddleware
{
    private readonly RequestDelegate _next;

    public HttpTenantIdentifierMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // only need to get the get the tenant if the path is not the refresh endpoint
        if (context.Request.Path != "/refresh-settings")
        {
            var tenantSettings = context.RequestServices.GetRequiredService<TenantSettings>();

            var multiTenantOptions = context.RequestServices.GetRequiredService<MultiTenantOptions>();

            if (context.TryExtractTenantFromHttpContext(multiTenantOptions, out var tenant))
            {
                tenantSettings.SetCurrentTenant(tenant);
            }

            if (tenantSettings.Tenant is null && multiTenantOptions.DefaultTenant is not null)
            {
                tenantSettings.SetCurrentTenant(multiTenantOptions.DefaultTenant);
            }

            if (tenantSettings.Tenant is null)
            {
                throw new Exception("Tenand not identified!");
            }
        }

        await _next(context);
    }
}
