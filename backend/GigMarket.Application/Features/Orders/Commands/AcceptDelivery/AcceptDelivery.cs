using MediatR;

namespace GigMarket.Application.Features.Orders.Commands.AcceptDelivery;

public sealed record AcceptDeliveryCommand(Guid OrderId) : IRequest;