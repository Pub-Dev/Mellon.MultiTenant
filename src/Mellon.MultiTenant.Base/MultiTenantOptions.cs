namespace Mellon.MultiTenant.Base;

using System.Net.Http.Json;
using Mellon.MultiTenant.Base.Enums;
using Mellon.MultiTenant.Base.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

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

	public Func<EndpointSettings, IConfiguration, Task<string[]>> GetTenantSourceHttpEndpointFunc { get; private set; }

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

	public MultiTenantOptions LoadFromEndpoint(Func<EndpointSettings, IConfiguration, Task<string[]>> func)
	{
		TenantSource = TenantSource.Endpoint;

		GetTenantSourceHttpEndpointFunc = func;

		return this;
	}

	public MultiTenantOptions LoadFromEndpoint<T>(Func<T, string> func) =>
		LoadFromEndpoint(async (endpointOptions, configuration) =>
		{
			var request = new HttpRequestMessage()
			{
				RequestUri = new Uri(endpointOptions.Url),
				Method = new HttpMethod(endpointOptions.Method ?? "GET"),
			};

			if (!string.IsNullOrEmpty(endpointOptions.Authorization))
			{
				request.Headers.Add("Authorization", endpointOptions.Authorization);
			}

			using var client = new HttpClient();

			var result = await client.SendAsync(request);

			if (result.IsSuccessStatusCode)
			{
				var data = await result.Content.ReadFromJsonAsync<IEnumerable<T>>();

				var tenants = data!.Select(func).ToArray();

				if (tenants.Length == 0)
				{
					throw new Exception($"No tenant found on the endpoint {endpointOptions.Url}");
				}

				return tenants;
			}
			else
			{
				var statusCode = result.StatusCode;

				var reason = result.ReasonPhrase;

				var content = await result.Content.ReadAsStringAsync();

				throw new Exception($@"Error to load tenants from the url {endpointOptions.Url} StatusCode: {statusCode} Reason: {reason} Content: {content}");
			}
		});

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

	public MultiTenantOptions WithSkipTenantCheckPaths(string path)
	{
		SkipTenantCheckPaths.Add(path);

		return this;
	}

	public MultiTenantOptions WithSkipTenantCheckPaths(params string[] path)
	{
		SkipTenantCheckPaths.AddRange(path);

		return this;
	}

	public MultiTenantOptions WithCustomTenantConfigurationSource<T>() where T : IMultiTenantConfigurationSource
	{
		CustomMultiTenantConfigurationSource = typeof(T);

		return this;
	}
}