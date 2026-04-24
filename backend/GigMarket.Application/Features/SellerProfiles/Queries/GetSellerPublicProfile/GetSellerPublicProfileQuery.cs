using MediatR;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetSellerPublicProfile;

public sealed record GetSellerPublicProfileQuery(Guid SellerProfileId)
    : IRequest<SellerPublicProfileDto>;