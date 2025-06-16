
using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelInspiration.API.Shared.Persistence;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Stops;

public sealed class GetStops :ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet(
            "api/itineraries/{itineraryId}/stops",
            (int itineraryId,
                CancellationToken cancellationToken, IMediator mediator) =>
            {
                

                return mediator.Send(new GetStopsQuery(itineraryId), cancellationToken);

            });
    }

    public sealed class GetStopsQuery(int itineraryId) : IRequest<IResult>
    {
        public int ItineraryId { get; init; } = itineraryId;
    }

    public sealed class GetStopsHandler(TravelInspirationDbContext dbContext) : IRequestHandler<GetStopsQuery, IResult>
    {
        public async Task<IResult> Handle(GetStopsQuery request, CancellationToken cancellationToken)
        {
            var itinerary = await dbContext.Itineraries
                .Include(i => i.Stops)
                .FirstOrDefaultAsync(i => i.Id == request.ItineraryId, cancellationToken);

            if (itinerary is null) return Results.NotFound();

            var result = itinerary.Stops.Select(stop => new StopDto
                {
                    Id=stop.Id,
                    Name=stop.Name,
                    ImageUri = stop.ImageUri,
                    ItineraryId=stop.ItineraryId,
                }
            );
            return Results.Ok(result.ToList());
        }
    }

    public sealed class StopDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public Uri? ImageUri { get; set; }
        public required int ItineraryId { get; set; }
    }
}