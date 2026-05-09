using MediatR;

namespace GigMarket.Application.Features.Orders.Commands.DeliverOrder;

public sealed record DeliverOrderCommand(
    Guid OrderId,
    string Message,
    List<string> FileUrls) : IRequest;