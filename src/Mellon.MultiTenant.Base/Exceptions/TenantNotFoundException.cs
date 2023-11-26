namespace Mellon.MultiTenant.Base.Exceptions;

internal class TenantNotFoundException(string message) :
    Exception($"Tenant {message} not found")
{
}
