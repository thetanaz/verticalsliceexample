using MediatR;
using Microsoft.EntityFrameworkCore;
using TravelInspiration.API.Shared.Persistence;
using TravelInspiration.API.Shared.Slices;

namespace TravelInspiration.API.Features.Itineraries;

public sealed class GetItineraries :ISlice
{
    public  void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        
        endpointRouteBuilder.MapGet("/api/itineraries",
             (string? searchFor,CancellationToken cancellationToken,IMediator mediator) =>
            {
               


                return mediator.Send(new GetItinerariesQuery(searchFor),cancellationToken);
            });
        
    }

    public sealed class GetItinerariesQuery(string? searchFor) :IRequest<IResult>
    {
        public string? SearchFor { get; init; } = searchFor;
    }

  


    public  sealed class GetItinerariesHandler(TravelInspirationDbContext dbContext):IRequestHandler<GetItinerariesQuery,IResult>
    {
        public async Task<IResult> Handle(GetItinerariesQuery request, CancellationToken cancellationToken)
        {
            var itineraries = await dbContext.Itineraries.Where(i =>
                    request.SearchFor == null || i.Name.Contains(request.SearchFor) ||
                    (i.Description != null && i.Description.Contains(request.SearchFor)))
                .ToListAsync(cancellationToken);

            var result = itineraries.Select(i => new ItineraryDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                UserId = i.UserId,
            }).ToList();
            return Results.Ok(result);
        }
    }

    public sealed class ItineraryDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string UserId { get; set; }
    }
}