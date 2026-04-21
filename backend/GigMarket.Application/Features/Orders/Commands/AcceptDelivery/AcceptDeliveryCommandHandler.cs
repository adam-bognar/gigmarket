using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Orders.Commands.AcceptDelivery;

public sealed class AcceptDeliveryCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<AcceptDeliveryCommand>
{
    public async Task Handle(AcceptDeliveryCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var order = await db.Orders
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct)
                    ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

        if (order.BuyerUserId != userId)
            throw new UnauthorizedException("Only the buyer can accept a delivery.");

        if (order.Status != OrderStatus.Delivered)
            throw new BadRequestException($"Cannot accept a delivery on an order with status '{order.Status}'.");

        order.Status = OrderStatus.Completed;
        await db.SaveChangesAsync(ct);
    }
}