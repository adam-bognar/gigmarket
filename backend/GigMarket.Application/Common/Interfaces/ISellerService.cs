using GigMarket.Application.Features.SellerProfiles.Commands.CreateSellerProfile;
using GigMarket.Application.Features.SellerProfiles.Models;

namespace GigMarket.Application.Common.Interfaces;

public interface ISellerService
{
    Task<SellerProfileDto> CreateSellerAsync(CreateSellerProfileRequest request, CancellationToken ct);
}