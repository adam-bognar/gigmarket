using GigMarket.Application.Features.Gigs.Commands.CreateGig;
using GigMarket.Application.Features.SellerProfiles.Models;

namespace GigMarket.Application.Common.Interfaces;

public interface IGigService
{
    Task<GigDto> CreateGigAsync(CreateGigRequest request, CancellationToken ct);
}