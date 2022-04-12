namespace Microsoft.Extensions.Diagnostics.HealthChecks.Elasticsearch;

/// <inheritdoc cref="IElasticsearchHealthCheckSettings"/>
public class ElasticsearchHealthCheckSettings : IElasticsearchHealthCheckSettings
{
    /// <inheritdoc />
    public string Url { get; set; } = null!;

    /// <inheritdoc />
    public string? Username { get; set; }

    /// <inheritdoc />
    public string? Password { get; set; }
}