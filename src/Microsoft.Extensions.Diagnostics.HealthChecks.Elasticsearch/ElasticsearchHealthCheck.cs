using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Diagnostics.HealthChecks.Elasticsearch;

internal class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly IElasticsearchHealthCheckSettings _settings;
    private readonly ILogger<ElasticsearchHealthCheck> _logger;

    public ElasticsearchHealthCheck(IElasticsearchHealthCheckSettings settings, ILogger<ElasticsearchHealthCheck> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.LogDebug("{Url} ; {Username} ; {Password}", _settings.Url, _settings.Username, _settings.Password);
        ServicePointManager.ServerCertificateValidationCallback += (_, _, _, _) => true;
        var httpClient = new HttpClient();
        var request = BuildRequest();
        try
        {
            var response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var description = $"Elastic responded with status code {(int) response.StatusCode}";
                _logger.LogError(description);
                return HealthCheckResult.Unhealthy(description);
            }

            var responseContent = await response.Content.ReadFromJsonAsync<HealthResponse>(cancellationToken: cancellationToken);
            _logger.LogInformation("{Status}", JsonSerializer.Serialize(responseContent));
            return responseContent?.Status switch
            {
                "green" => HealthCheckResult.Healthy(),
                "yellow" => HealthCheckResult.Degraded(),
                "red" => HealthCheckResult.Unhealthy(),
                _ => throw new ArgumentOutOfRangeException(nameof(HealthResponse.Status), responseContent?.Status ?? "<null>")
            };
        }
        catch (Exception e)
        {
            _logger.LogError("Elastic didn't respond correctly");
            return HealthCheckResult.Unhealthy(e.Message);
        }
    }

    private HttpRequestMessage BuildRequest()
    {
        var url = new string(_settings.Url).TrimEnd('/');
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"{url}/_cluster/health?format=json")
        };
        if (_settings.Username is not null && _settings.Password is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", BasicAuthHeaderValue());
        return request;
    }

    private string BasicAuthHeaderValue() =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.Username}:{_settings.Password}"));

    private class HealthResponse
    {
        public string Status { get; set; } = null!;
    }
}