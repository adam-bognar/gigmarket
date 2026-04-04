using MediatR;

namespace GigMarket.Application.Features.Orders.Commands.CreateCheckoutSession;

public sealed record CreateCheckoutSessionCommand(Guid GigId, Guid PackageId) : IRequest<string>;