using Mellon.MultiTenant.Base.Enums;
using Microsoft.AspNetCore.Http;

namespace Mellon.MultiTenant.Base;

public class MultiTenantOptions
{
    public TenantSource TenantSource { get; set; } = TenantSource.AppSettings;

    public string ApplicationName { get; set; }

    public string HttpHeaderKey { get; set; }

    public string QueryStringKey { get; set; }

    public string CookieKey { get; set; }

    public string DefaultTenant { get; set; }

    public Func<HttpContext, string> GetTenantFromHttClientFunc { get; private set; }

    public MultiTenantOptions LoadFromAppSettings()
    {
        TenantSource = TenantSource.AppSettings;
        return this;
    }

    public MultiTenantOptions LoadFromEnvironmentVariable()
    {
        TenantSource = TenantSource.EnvironmentVariables;
        return this;
    }

    public MultiTenantOptions WithApplicationName(string applicationName)
    {
        ApplicationName = applicationName;
        return this;
    }

    public MultiTenantOptions WithHttpHeader(string httpHeader)
    {
        HttpHeaderKey = httpHeader;
        return this;
    }

    public MultiTenantOptions WithQueryString(string queryString)
    {
        QueryStringKey = queryString;
        return this;
    }

    public MultiTenantOptions WithCookie(string cookieName)
    {
        CookieKey = cookieName;
        return this;
    }

    public MultiTenantOptions WithHttpContextLoad(Func<HttpContext, string> func)
    {
        GetTenantFromHttClientFunc = func;
        return this;
    }

    public MultiTenantOptions WithDefaultTenant(string defaultTenant)
    {
        DefaultTenant = defaultTenant;
        return this;
    }
}
