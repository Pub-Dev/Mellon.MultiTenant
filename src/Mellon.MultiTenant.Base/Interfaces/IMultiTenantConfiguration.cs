namespace Mellon.MultiTenant.Base.Interfaces;

using Microsoft.Extensions.Configuration;

public interface IMultiTenantConfiguration : IConfiguration
{
	public string Tenant { get; }
}