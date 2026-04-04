using MediatR;

namespace GigMarket.Application.Features.Orders.Commands.FulfillOrder;

public sealed record FulfillOrderCommand(
    string StripeSessionId,
    Guid GigId,
    Guid PackageId,
    Guid BuyerUserId,
    decimal TotalPrice,
    DateTime PaidAtUtc) : IRequest;