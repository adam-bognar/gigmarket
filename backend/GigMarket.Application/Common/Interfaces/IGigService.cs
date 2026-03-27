using GigMarket.Application.Features.Gigs.Commands.CreateGig;
using GigMarket.Application.Features.Gigs.Commands.UpdateGig;
using GigMarket.Application.Features.SellerProfiles.Models;

namespace GigMarket.Application.Common.Interfaces;

public interface IGigService
{
    Task<GigDto> CreateGigAsync(CreateGigRequest request, CancellationToken ct);
    Task<GigDto> UpdateGigAsync(Guid gigId, UpdateGigRequest request, CancellationToken ct);
    Task DeleteGigAsync(Guid gigId, CancellationToken ct);
}