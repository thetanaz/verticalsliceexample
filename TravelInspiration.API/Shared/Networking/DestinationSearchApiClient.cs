using Polly;
using Polly.Retry;
using System.Net.Http.Headers;
using System.Text.Json;
using TravelInspiration.API.Shared.Domain.Models;

namespace TravelInspiration.API.Shared.Networking;

public class DestinationSearchApiClient : IDestinationSearchApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public DestinationSearchApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;

        _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(msg => !msg.IsSuccessStatusCode) // optional: retry on bad status codes too
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        _jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    public async Task<IEnumerable<Destination>> GetDestinationsAsync(string? searchFor, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var baseUrl = _configuration["Integrations:DestinationSearchApiRoot"];
        var requestUrl = $"{baseUrl}destinations?searchFor={searchFor}";

        var response = await _retryPolicy.ExecuteAsync(async () =>
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        });

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<IEnumerable<Destination>>(stream, _jsonSerializerOptions, cancellationToken) ?? [];
    }
}
