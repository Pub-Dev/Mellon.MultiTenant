namespace Mellon.MultiTenant.Middlewares;

using System.Text.RegularExpressions;
using Mellon.MultiTenant.Base;
using Mellon.MultiTenant.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

public partial class HttpTenantIdentifierMiddleware
{
	private readonly RequestDelegate _next;

	private readonly List<Regex> _regexes = [];

	public HttpTenantIdentifierMiddleware(
		RequestDelegate next,
		MultiTenantOptions multiTenantOptions)
	{
		_next = next;

		if (multiTenantOptions.SkipTenantCheckPaths != null && multiTenantOptions.SkipTenantCheckPaths.Count > 0)
		{
			LoadRegexes(multiTenantOptions.SkipTenantCheckPaths);
		}

		_regexes.Add(RefreshSettingsRegex());
	}

	public Task InvokeAsync(HttpContext context)
	{
		var tenantSettings = context.RequestServices.GetRequiredService<TenantSettings>();

		var multiTenantOptions = context.RequestServices.GetRequiredService<MultiTenantOptions>();

		if (context.TryExtractTenantFromHttpContext(multiTenantOptions, out var tenant))
		{
			tenantSettings.SetCurrentTenant(tenant);
		}

		if (tenantSettings.Tenant is null && multiTenantOptions.DefaultTenant is not null)
		{
			tenantSettings.SetCurrentTenant(multiTenantOptions.DefaultTenant);
		}

		// only need to throw an exception if the path is not the refresh endpoint or path is whitelisted when the tenant is not defined
		if (!IsWhiteListedPath(context.Request.Path))
		{
			if (tenantSettings.Tenant is null)
			{
				throw new Exception("Tenant not identified!");
			}
		}

		return _next(context);
	}

	[GeneratedRegex("^/refresh-settings.*", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
	private static partial Regex RefreshSettingsRegex();

	private void LoadRegexes(List<string> paths)
	{
		foreach (var item in paths)
		{
			var regex = new Regex(item, RegexOptions.Compiled, TimeSpan.FromSeconds(1));

			_regexes.Add(regex);
		}
	}

	private bool IsWhiteListedPath(string path)
	{
		if (_regexes.Count == 0)
		{
			return true;
		}

		foreach (var regex in _regexes)
		{
			if (regex.IsMatch(path))
			{
				return true;
			}
		}

		return false;
	}
}