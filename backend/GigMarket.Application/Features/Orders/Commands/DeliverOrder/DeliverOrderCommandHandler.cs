using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Orders.Commands.DeliverOrder;

public sealed class DeliverOrderCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<DeliverOrderCommand>
{
    public async Task Handle(DeliverOrderCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var order = await db.Orders
                        .Include(o => o.Gig)
                        .ThenInclude(g => g.SellerProfile)
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct)
                    ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

        if (order.Gig.SellerProfile.UserId != userId)
            throw new UnauthorizedException("Only the seller can deliver this order.");

        if (order.Status is not (OrderStatus.InProgress or OrderStatus.UnderRevision))
            throw new BadRequestException($"Cannot deliver an order with status '{order.Status}'.");

        var fileUrls = request.FileUrls;

        var delivery = new OrderDelivery
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            Message = request.Message,
            CreatedAtUtc = DateTime.UtcNow,
            Attachments = fileUrls
                .Select((url, index) => new OrderDeliveryAttachment
                {
                    Id = Guid.NewGuid(),
                    FileUrl = url,
                    SortOrder = index,
                    CreatedAtUtc = DateTime.UtcNow
                })
                .ToList()
        };

        db.OrderDeliveries.Add(delivery);

        order.Status = OrderStatus.Delivered;

        await db.SaveChangesAsync(ct);
    }
}