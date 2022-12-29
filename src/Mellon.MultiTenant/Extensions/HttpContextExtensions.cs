using Microsoft.AspNetCore.Http;

namespace Mellon.MultiTenant.Extensions
{
    internal static class HttpContextExtensions
    {
        internal static bool TryExtractTenantFromHttpContext(
            this HttpContext context,
            MultiTenantOptions multiTenantOptions,
            out string tenant)
        {
            if (multiTenantOptions.GetTenantFromHttClientFunc is not null)
            {
                tenant = multiTenantOptions.GetTenantFromHttClientFunc(context);

                return true;
            }

            if (multiTenantOptions.HttpHeaderKey is not null &&
               context.Request.Headers.TryGetValue(multiTenantOptions.HttpHeaderKey, out var header))
            {
                tenant = header;

                return true;
            }

            if (multiTenantOptions.QueryStringKey is not null &&
                context.Request.Query.TryGetValue(multiTenantOptions.QueryStringKey, out var queryString))
            {
                tenant = queryString;

                return true;
            }

            if (multiTenantOptions.CookieKey is not null &&
                context.Request.Cookies.TryGetValue(multiTenantOptions.CookieKey, out var cookie))
            {
                tenant = cookie;

                return true;
            }

            tenant = null;

            return false;
        }
    }
}
