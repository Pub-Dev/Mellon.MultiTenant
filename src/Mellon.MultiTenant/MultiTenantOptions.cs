using Mellon.MultiTenant.Enums;

namespace Mellon.MultiTenant;

public class MultiTenantOptions
{
    public ConfigurationSource ConfigurationSource { get; set; } = ConfigurationSource.Local;
    public TenantSource TenantSource { get; set; } = TenantSource.AppSettings;

    public string ApplicationName { get; set; }

    public string? HttpHeaderKey { get; set; }

    public string? QueryStringKey { get; set; }

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

    public MultiTenantOptions WithConfigurationSource(ConfigurationSource configurationSource)
    {
        ConfigurationSource = configurationSource;

        return this;
    }
}
