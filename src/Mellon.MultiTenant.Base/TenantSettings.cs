namespace Mellon.MultiTenant.Base;

using Mellon.MultiTenant.Base.Exceptions;
using Microsoft.Extensions.Configuration;

public class TenantSettings(MultiTenantSettings multiTenantSettings)
{
	private readonly MultiTenantSettings _multiTenantSettings = multiTenantSettings;

	public string Tenant { get; private set; }

	public IConfiguration Configuration => Tenant is null ? null : _multiTenantSettings.GetConfigurations[Tenant];

	public void SetCurrentTenant(string tenant)
	{
		if (_multiTenantSettings.GetConfigurations.ContainsKey(tenant))
		{
			Tenant = tenant;
		}
		else
		{
			throw new TenantNotFoundException(tenant);
		}
	}
}