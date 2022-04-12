An extension to ASP.net core health check system that allows health checking an Elasticsearch cluster

## Getting started

A Nuget package is available [here](https://www.nuget.org/packages/Eonix.Microsoft.Extensions.Diagnostics.HealthChecks.Elasticsearch). It can be installed using the Nuget package manager or the `dotnet` CLI.

`dotnet add Eonix.Microsoft.Extensions.Diagnostics.HealthChecks.Elasticsearch`

## Example

In `Program` , add the following services registration:

```csharp
builder.Services.AddHealthChecks()
    .AddElasticsearchHealthCheck(settings =>
    {
        settings.Url = "<your-cluster-url";
        settings.Username = "<basic authentication username>";
        settings.Password = "<basic authentication password>";
    });
```

This library only supports Elastic basic authentication so far. It is optional; if no username nor password are provided, then no basic authentication header will be added.

You can also pass a `IConfiguration` object:

```csharp
builder.Services.AddHealthChecks()
    .AddElasticsearchHealthCheck(builder.Configuration.GetSection("Elastic"));
```

The configuration must have the following structure:

```json
  "Elastic": {
    "Url": "<your-cluster-url",
    "Username": "<basic authentication username>",
    "Password": "<basic authentication password>"
  }
```

Finally, you can also use your own custom settings class (in case you need to use elastic settings somewhere else). Your class must implement `IElasticsearchHealthCheckSettings`. Be sure to register your settings class with the [options pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-6.0).

Then, in `Program`:

```csharp
builder.Services.AddHealthChecks()
    .AddElasticsearchHealthCheck<AnotherElasticSettingsClass>();
```