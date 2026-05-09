using MediatR;
using GigMarket.Application.Features.SellerProfiles.Models;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetSellerEarnings;

public sealed record GetSellerEarningsQuery : IRequest<SellerEarningsDto>;