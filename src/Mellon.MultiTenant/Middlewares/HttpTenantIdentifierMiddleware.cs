using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Mellon.MultiTenant.Middlewares;

public class HttpTenantIdentifierMiddleware
{
    private const string TENAND_KEY_NAME = "x-tenant-name";

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

            StringValues tenant = default;

            if (multiTenantOptions.HttpHeaderKey is not null &&
                context.Request.Headers.TryGetValue(multiTenantOptions.HttpHeaderKey, out tenant))
            {
                tenantSettings.SetTenant(tenant);
            }
            else if (multiTenantOptions.QueryStringKey is not null &&
                context.Request.Query.TryGetValue(multiTenantOptions.QueryStringKey, out tenant))
            {
                tenantSettings.SetTenant(tenant);
            }

            if (tenantSettings.Tenant is null)
            {
                throw new Exception("Tenand not identified!");
            }
        }

        await _next(context);
    }
}
