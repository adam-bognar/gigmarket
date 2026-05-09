using MediatR;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetStripeDashboardLink;

public sealed record GetStripeDashboardLinkQuery : IRequest<string>;