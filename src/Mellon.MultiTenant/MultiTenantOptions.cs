using Mellon.MultiTenant.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Mellon.MultiTenant;

public class MultiTenantOptions
{
    public ConfigurationSource ConfigurationSource { get; set; } = ConfigurationSource.Local;

    public TenantSource TenantSource { get; set; } = TenantSource.AppSettings;

    public string ApplicationName { get; set; }

    public string HttpHeaderKey { get; set; }

    public string QueryStringKey { get; set; }

    public string CookieKey { get; set; }

    public string AzureAppConfigurationConnectionString { get; set; }

    public string DefaultTenant { get; set; }

    internal Func<HttpContext, string> GetTenantFromHttClientFunc;

    internal Func<string, Action<AzureAppConfigurationOptions>> AzureAppConfigurationOptions;

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

    public MultiTenantOptions WithConfigurationSource(ConfigurationSource configurationSource)
    {
        ConfigurationSource = configurationSource;

        return this;
    }

    public MultiTenantOptions WithLocalConfiguration()
    {
        ConfigurationSource = ConfigurationSource.Local;

        return this;
    }

    public MultiTenantOptions WithSpringCloudConfiguration()
    {
        ConfigurationSource = ConfigurationSource.SpringCloud;

        return this;
    }

    public MultiTenantOptions WithAzureAppConfiguration(Func<string, Action<AzureAppConfigurationOptions>> options)
    {
        AzureAppConfigurationOptions = options;

        return WithConfigurationSource(ConfigurationSource.Azure);
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
