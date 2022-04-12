using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.Diagnostics.HealthChecks.Elasticsearch;

public static class ElasticsearchHealthCheckExtensions
{
    /// <summary>
    /// Enables Elasticsearch health check.
    /// </summary>
    /// <param name="healthChecksBuilder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="configureSettings">The action used to configure the health check settings (see <see cref="ElasticsearchHealthCheckSettings"/>).</param>
    /// <param name="name">The health check name.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    public static IHealthChecksBuilder AddElasticsearchHealthCheck(this IHealthChecksBuilder healthChecksBuilder,
        Action<ElasticsearchHealthCheckSettings> configureSettings, string name = ElasticsearchHealthCheckDefaults.HealthCheckName)
    {
        healthChecksBuilder.Services.Configure(configureSettings);
        return healthChecksBuilder.AddElasticsearchHealthCheck<ElasticsearchHealthCheckSettings>(name);
    }

    /// <summary>
    /// Enables Elasticsearch health check.
    /// </summary>
    /// <param name="healthChecksBuilder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="elasticHealthCheckConfiguration">The configuration section being bound to the health check settings (see <see cref="ElasticsearchHealthCheckSettings"/>).</param>
    /// <param name="name">The health check name.</param>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    public static IHealthChecksBuilder AddElasticsearchHealthCheck(this IHealthChecksBuilder healthChecksBuilder, 
        IConfiguration elasticHealthCheckConfiguration, string name = ElasticsearchHealthCheckDefaults.HealthCheckName)
    {
        healthChecksBuilder.Services.Configure<ElasticsearchHealthCheckSettings>(elasticHealthCheckConfiguration);
        return healthChecksBuilder.AddElasticsearchHealthCheck<ElasticsearchHealthCheckSettings>(name);
    }
    
    /// <summary>
    /// Enables Elasticsearch health check. Use this overload if you want to use your own Elasticsearch settings class.
    /// </summary>
    /// <param name="healthChecksBuilder">The <see cref="IHealthChecksBuilder"/>.</param>
    /// <param name="name">The health check name.</param>
    /// <typeparam name="TSettings">The type of the Elasticsearch health check settings class to use. Must implement <see cref="IElasticsearchHealthCheckSettings"/>.</typeparam>
    /// <returns>The <see cref="IHealthChecksBuilder"/>.</returns>
    public static IHealthChecksBuilder AddElasticsearchHealthCheck<TSettings>(this IHealthChecksBuilder healthChecksBuilder, 
        string name = ElasticsearchHealthCheckDefaults.HealthCheckName) 
        where TSettings : class, IElasticsearchHealthCheckSettings
    {
        healthChecksBuilder.Services.AddTransient<IElasticsearchHealthCheckSettings, TSettings>(sp => sp.GetRequiredService<IOptions<TSettings>>().Value);
        healthChecksBuilder.AddCheck<ElasticsearchHealthCheck>(name);
        return healthChecksBuilder;
    }
}