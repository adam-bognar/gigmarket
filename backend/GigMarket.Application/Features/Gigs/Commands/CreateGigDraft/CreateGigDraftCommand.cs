using GigMarket.Application.Features.SellerProfiles.Models;
using MediatR;

namespace GigMarket.Application.Features.Gigs.Commands.CreateGigDraft;

public record CreateGigDraftCommand : IRequest<GigDto>;