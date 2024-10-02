namespace Mellon.MultiTenant.Base;

using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

public class TenantConfiguration(
	TenantSettings tenantSettings,
	IConfiguration configuration) : IMultiTenantConfiguration
{
	public string Tenant { get; } = tenantSettings.Tenant;

	public IConfiguration Configuration { get; } = tenantSettings.Configuration ?? configuration;

	public string this[string key] { get => Configuration[key]; set => Configuration[key] = value; }

	public IEnumerable<IConfigurationSection> GetChildren() => Configuration.GetChildren();

	public IChangeToken GetReloadToken() => Configuration.GetReloadToken();

	public IConfigurationSection GetSection(string key) => Configuration.GetSection(key);
}