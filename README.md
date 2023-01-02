[![Contributors][contributors-shield]][contributors-url] [![Forks][forks-shield]][forks-url] [![Stargazers][stars-shield]][stars-url] [![Issues][issues-shield]][issues-url] [![LinkedIn][linkedin-shield]][linkedin-url] [![LinkedIn][linkedin-shield]][linkedin2-url]

## Mellon.MultiTenant by [@PubDev](https://www.youtube.com/@PubDev)

|               Package               |                                                                  Version                                                                   |                                                                     Alpha                                                                     |
| :---------------------------------: | :----------------------------------------------------------------------------------------------------------------------------------------: | :-------------------------------------------------------------------------------------------------------------------------------------------: |
|       **Mellon-MultiTenant**        |              [![Nuget](https://img.shields.io/nuget/v/Mellon-MultiTenant)](https://www.nuget.org/packages/Mellon-MultiTenant)              |              [![Nuget](https://img.shields.io/nuget/vpre/Mellon-MultiTenant)](https://www.nuget.org/packages/Mellon-MultiTenant)              |
| **Mellon-MultiTenant-ConfigServer** | [![Nuget](https://img.shields.io/nuget/v/Mellon-MultiTenant-ConfigServer)](https://www.nuget.org/packages/Mellon-MultiTenant-ConfigServer) | [![Nuget](https://img.shields.io/nuget/vpre/Mellon-MultiTenant-ConfigServer)](https://www.nuget.org/packages/Mellon-MultiTenant-ConfigServer) |
|    **Mellon-MultiTenant-Azure**     |        [![Nuget](https://img.shields.io/nuget/v/Mellon-MultiTenant-Azure)](https://www.nuget.org/packages/Mellon-MultiTenant-Azure)        |        [![Nuget](https://img.shields.io/nuget/vpre/Mellon-MultiTenant-Azure)](https://www.nuget.org/packages/Mellon-MultiTenant-Azure)        |

[![GitHublicense](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/1bberto/Mellon.MultiTenant/main/LICENSE) [![CI](https://github.com/1bberto/Mellon.MultiTenant/actions/workflows/buildAndPush.yml/badge.svg?branch=main)](https://github.com/1bberto/Mellon.MultiTenant/actions/workflows/buildAndPush.yml)

Why Mellon, mellon is the Sindarin (and Noldorin) word for "friend", yes I'm a big fan of LoR, so let's be friends?

## About The Project

This library was created to supply a set of tools to enable the creation of multi-tenant applications using .net.

### Built With

- [net6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [steeltoe](https://docs.steeltoe.io/api/v3/configuration)
- [Azure App Configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview)
- [Spring Cloud Config](https://cloud.spring.io/spring-cloud-config/reference/html)
- The most important, Love ❤️

## Getting Started

## Installation

With package Manager:

```
Install-Package Mellon-MultiTenant
```

With .NET CLI:

```
dotnet add package Mellon-MultiTenant
```

### Configurations

There are two ways to configure the settings, via config and through the api

#### Settings

```json
"MultiTenant": {
    "ApplicationName": "customer-api",
    "HttpHeaderKey": "x-tenant-name",
    "CookieKey": "tenant-name",
    "QueryStringKey": "tenant-name",
    "TenantSource": "Settings",
    "Tenants": [
      "client-a",
      "client-b",
      "client-c"
    ]
}
```

|     Property      |                                                                        Description                                                                         |             Default              |
| :---------------: | :--------------------------------------------------------------------------------------------------------------------------------------------------------: | :------------------------------: |
|  ApplicationName  |                                                                      Application name                                                                      | IHostEnvironment.ApplicationName |
|   HttpHeaderKey   |                                                   HTTP Header key, where the tenant name will be passed                                                    |              `null`              |
|     CookieKey     |                                                   HTTP Cookie key, where the tenant name will be passed                                                    |              `null`              |
|  QueryStringKey   |                                                HTTP Query String key, where the tenant name will be passed                                                 |              `null`              |
|   TenantSource    |                    Where the list of possible tenants will be stored, it can be from two sources: `Settings` or `EnvironmentVariables`                     |            `Settings`            |
|      Tenants      |                            When the property `TenantSource` is set to `Settings` this property must contain the list of tenants                            |              `null`              |
| WithDefaultTenant | When the tenant is not defined by the caller the lib will set the tenant as the tenant defined within this property, use it just when actually needed 😉👍 |              `null`              |

When `TenantSource` is set to `EnvironmentVariables` it will get the tenant list from the environment variable `MULTITENANT_TENANTS`, this environment variable must contain the list of possible tenants in a single string, separating the tenants using `,`
for example:

```
$Env:MULTITENANT_TENANTS = 'client-a,client-b,client-c'
```

### Using the API

You can also set the settings using these options while you are adding the services

```csharp
builder.Services
        .AddMultiTenant(options =>
            options
                .WithApplicationName("customer-api")
                .WithHttpHeader("x-tenant-name")
                .WithCookie("tenant-name")
                .WithQueryString("tenant-name")
                .WithDefaultTenant("client-a")
                .LoadFromAppSettings()
        );
```

#### `WithApplicationName(string)`

- Set the application name

#### `WithHttpHeader(string)`

- Set the HTTP Header key, where the tenant name will be passed

#### `WithCookie(string)`

- Set the HTTP Cookie key, where the tenant name will be passed

#### `WithQueryString(string)`

- Set the HTTP Query String key, where the tenant name will be passed

#### `WithDefaultTenant(string)`

- Set for when the tenant is not defined by the caller the lib will set the tenant as the tenant defined within this property, use it just when needed 😉👍

#### `LoadFromSettings`

- Set for when the tenant list will be loaded from the settings **MultiTenant:Tenants**

#### `LoadFromEnvironmentVariable`

- Set for when the tenant list will be loaded from the environment variable **MULTITENANT_TENANTS**

#### `WithHttpContextLoad(Func<HttpContext, string> func)`

- When all the possibilities above do not meet your need you can create a custom "Middleware" to identify the tenant based on a `HttpContext`

#### `WithCustonTenantConfigurationSource<T>()`

- `T` must be an implementation of the interface `ITenantConfigurationSourceWhen` use it to define new a source of configurations for the tenants, for example, if the tenant settings are stored on XML files you could create something like this:

```csharp
public class LocalXmlTenantSource : ITenantConfigurationSource
{
    private readonly IHostEnvironment _hostEnvironment;

    public LocalTenantSource(
        IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public IConfigurationBuilder AddSource(
        string tenant,
        IConfigurationBuilder builder)
    {
        builder.AddXmlFile($"appsettings.{tenant}.xml", true);
        builder.AddJsonFile($"appsettings.{tenant}.{_hostEnvironment.EnvironmentName}.xml", true);

        return builder;
    }
}
```

### Local

This is the default source of settings for the tenants, there is no need to enable it, it will search for the settings on the application following this pattern:

- `appsettings.{tenant}.json`
- `appsettings.{_hostEnvironment.EnvironmentName}.json`

It is also worth mentioning that the configurations will also contain:

- `appsettings.json`
- `appsettings.[environment].json`
- `environment variables`

### Spring Cloud Config

You can also load the settings from a **Spring Cloud Config Server**!

To enable the usage you need to install an extra package:

With package Manager:

```
Install-Package Mellon-MultiTenant-ConfigServer
```

With .NET CLI:

```
dotnet add package Mellon-MultiTenant-ConfigServer
```

Once the package is installed you need to configure its services

```csharp
builder.Services
        .AddMultiTenant()
        .AddMultiTenantSpringCloudConfig();
```

To setup [Spring Cloud Config](https://cloud.spring.io/spring-cloud-config/reference/html) on your environment check this reporitory [DotNet-ConfigServer](https://github.com/Pub-Dev/Lesson-DotNet-ConfigServer)

The application name for spring cloud config will be based on the settings _**MultiTenant.ApplicationName**_ and the label will be tenant name.

Example:
_customer-api-client-a.yaml_

being:

- _customer-api_ the application name
- _client-a_ the tenant name

Moreover, it is worth mentioning that the settings for each customer will also have the settings of the current files:

- appsettings.json
- appsettings.[environment].json
- environment variables

### Azure App Configuration

You can also use it as a source of configuration the **Azure App Configuration**

With package Manager:

```
Install-Package Mellon-MultiTenant-Azure
```

With .NET CLI:

```
dotnet add package Mellon-MultiTenant-Azure
```

Once the package is installed you need to configure its services

```csharp
builder.Services
        .AddMultiTenant()
        .AddMultiTenantAzureAppConfiguration();
```

### `AddMultiTenantAzureAppConfiguration(Action<AzureMultiTenantOptions> action = null)`

if the action is not passed, the connection string used to connect on azure will be loaded from `AzureAppConfigurationConnectionString`

if you want to elaborate more, on how you are going to connect on Azure, you can use the `AzureMultiTenantOptions`, there is a property, which is a `Func<IServiceProvider, string, Action<AzureAppConfigurationOptions>>`, where the first parameter is the ServiceProvider, where you can extract the services; a string, being the tenant name; and the return of this `Func` must be an `Action<AzureAppConfigurationOptions>`.
For example:

```csharp
builder.Services
        .AddMultiTenant()
        .AddMultiTenantAzureAppConfiguration(options =>
            options.AzureAppConfigurationOptions = (serviceProvider, tenant) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                return azureOptions => azureOptions
                    .Connect(configuration["AzureAppConfigurationConnectionString"])
                    .Select("*", tenant);
            }
        );
```

## Usage / Samples

You can find some examples of how to use this library in the folder `/samples`

### Web API

To enable it on your api you first need to add the services:

```csharp
builder.Services.AddMultiTenant();
```

then you need also to register the middleware used to identify the tenant based on the `HttpContext`

```csharp
app.UseMultiTenant();
```

Once that is done you will be able to use the interface `IMultiTenantConfiguration`, this interface will behave the same as the `IConfiguration` interface, but contain only the current tenant settings:

Example:

```csharp
app.MapGet("/", (IMultiTenantConfiguration configuration) =>
{
    return new
    {
        Tenant = configuration.Tenant,
        Message = configuration["Message"],
    };
});
```

### EF Core Migrations:

To use it with EF Core is quite simple, you need to use the interface `IMultiTenantConfiguration` as mentioned above to setup your EF Context

#### Setup

```csharp
builder.Services.AddDbContext<DataBaseContext>(
    (IServiceProvider serviceProvider, DbContextOptionsBuilder options) =>
    {
        var configuration = serviceProvider.GetRequiredService<IMultiTenantConfiguration>();

        options.UseSqlServer(configuration?["ConnectionStrings:DefaultConnection"]);
    });
```

#### Migrations

To apply the migrations, you only need to do this:

```csharp
var tenants = app.Services.GetRequiredService<MultiTenantSettings>();

foreach (var tenant in tenants.Tenants)
{
    using (var scope = app.Services.CreateScope())
    {
        var tenantSettings = scope.ServiceProvider.GetRequiredService<TenantSettings>();

        tenantSettings.SetCurrentTenant(tenant);

        var db = scope.ServiceProvider.GetRequiredService<DataBaseContext>();

        await db.Database.MigrateAsync();
    }
}

app.Run();
```

## Extras

We know that settings can be changed all the time, but to get our applications running on with the latest settings we need to restart the application, it caused downtime and it's not very practical. Keeping that in mind, we added also an endpoint that when called will refresh all the settings for all the tenants or a specific tenant:

- `/refresh-settings`
- `/refresh-settings/{tenantName}`

PS: this will work only with AzureAppConfiguration and SpringCloudConfig

## Roadmap

- Add unit tests 🧪
- Add new Config Source
- Load the Tenants from a web-api request
- Enable the usage with HanfFire

See the [open issues](https://github.com/1bberto/Mellon.MultiTenant/issues) for a full list of proposed features (and known issues).

## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Contact

- Humberto Rodrigues - [@1bberto](https://instagram.com/1bberto) - humberto_henrique1@live.com
- Rafael Nagai - [@naganaga](https://instagram.com/rafakenji23) - rafakenji23@gmail.com

Project Link: [https://github.com/1bberto/Mellon.MultiTenant](https://github.com/1bberto/Mellon.MultiTenant)

[contributors-shield]: https://img.shields.io/github/contributors/1bberto/Mellon.MultiTenant.svg?style=for-the-badge
[contributors-url]: https://github.com/1bberto/Mellon.MultiTenant/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/1bberto/Mellon.MultiTenant.svg?style=for-the-badge
[forks-url]: https://github.com/1bberto/Mellon.MultiTenant/network/members
[stars-shield]: https://img.shields.io/github/stars/1bberto/Mellon.MultiTenant.svg?style=for-the-badge
[stars-url]: https://github.com/1bberto/Mellon.MultiTenant/stargazers
[issues-shield]: https://img.shields.io/github/issues/1bberto/Mellon.MultiTenant.svg?style=for-the-badge
[issues-url]: https://github.com/1bberto/Mellon.MultiTenant/issues
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/humbberto
[linkedin2-url]: https://br.linkedin.com/in/rafakenji
