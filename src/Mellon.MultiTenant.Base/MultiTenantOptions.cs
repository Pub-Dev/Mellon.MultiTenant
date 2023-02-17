using Mellon.MultiTenant.Base.Enums;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Mellon.MultiTenant.Base;

public class MultiTenantOptions
{
    public TenantSource TenantSource { get; set; } = TenantSource.Settings;

    public string ApplicationName { get; set; }

    public string HttpHeaderKey { get; set; }

    public string QueryStringKey { get; set; }

    public string CookieKey { get; set; }

    public string DefaultTenant { get; set; }

    public List<string> SkipTenantCheckPaths { get; set; }

    public Func<HttpContext, string> GetTenantFromHttClientFunc { get; private set; }

    public Type CustomMultiTenantConfigurationSource { get; private set; }

    public Func<EndpointSettings, IConfiguration, string[]> GetTenantSourceHttpEndpointFunc { get; private set; }

    public EndpointSettings Endpoint { get; set; }

    public MultiTenantOptions LoadFromSettings()
    {
        TenantSource = TenantSource.Settings;

        return this;
    }

    public MultiTenantOptions LoadFromEnvironmentVariable()
    {
        TenantSource = TenantSource.EnvironmentVariables;

        return this;
    }

    public MultiTenantOptions LoadFromEndpoint(Func<EndpointSettings, IConfiguration, string[]> func)
    {
        TenantSource = TenantSource.Endpoint;

        GetTenantSourceHttpEndpointFunc = func;

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

    public MultiTenantOptions WithCustomTenantConfigurationSource<T>() where T : ITenantConfigurationSource
    {
        CustomMultiTenantConfigurationSource = typeof(T);

        return this;
    }

    public class EndpointSettings
    {
        public string Url { get; set; }
        public string Authorization { get; set; }
    }
}
