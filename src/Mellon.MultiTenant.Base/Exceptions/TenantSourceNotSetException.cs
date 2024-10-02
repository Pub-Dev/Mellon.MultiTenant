namespace Mellon.MultiTenant.Base.Exceptions;

using Mellon.MultiTenant.Base.Enums;

internal class TenantSourceNotSetException(TenantSource source) :
	Exception($"{source} not set!")
{
}