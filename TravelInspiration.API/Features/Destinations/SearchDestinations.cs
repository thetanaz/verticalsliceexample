using TravelInspiration.API.Shared.Networking;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Destinations;

public sealed class SearchDestinations :ISlice
{
    public  void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("api/destinations", async (string? searchFor, ILoggerFactory logger,
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