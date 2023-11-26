using Mellon.MultiTenant.Base.Enums;

namespace Mellon.MultiTenant.Base.Exceptions;

internal class TenantSourceNotSetException(TenantSource source) :
    Exception($"{source} not set!")
{
}

