using TravelInspiration.API.Shared.Networking;

namespace TravelInspiration.API.Features.SearchDestinations;

public static class SearchDestinations
{
    public static void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/destinations", async (string? searchFor, ILoggerFactory logger,
            IDestinationSearchApiClient destinationSearchApiClient, CancellationToken cancellationToken) =>
        {
            logger.CreateLogger("EndpointHandlers").LogInformation("Search destinations feature called");
            
            var resultFromApiCall = await destinationSearchApiClient.GetDestinationsAsync(searchFor, cancellationToken);

            var result = resultFromApiCall.Select(d => new
            {
              d.Name,
              d.Description,
              d.ImageUri
            });
            
            return Results.Ok(result);
        });
    }
}