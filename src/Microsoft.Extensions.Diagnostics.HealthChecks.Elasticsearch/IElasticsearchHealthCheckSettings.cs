namespace Microsoft.Extensions.Diagnostics.HealthChecks.Elasticsearch;

public interface IElasticsearchHealthCheckSettings
{
    /// <summary>
    /// The base url of the Elasticsearch cluster. Mandatory.
    /// </summary>
    string Url { get; }
    
    /// <summary>
    /// The username used for basic authentication. Leave null if no authentication is required.
    /// </summary>
    string? Username { get; }
    
    /// <summary>
    /// The password used for basic authentication. Leave null if no authentication is required.
    /// </summary>
    string? Password { get; }
}