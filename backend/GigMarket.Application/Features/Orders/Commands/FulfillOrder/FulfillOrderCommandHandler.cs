using GigMarket.Application.Common.Interfaces;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Orders.Commands.FulfillOrder;

public sealed class FulfillOrderCommandHandler(IApplicationDbContext db)
    : IRequestHandler<FulfillOrderCommand>
{
    public async Task Handle(FulfillOrderCommand request, CancellationToken ct)
    {
        var alreadyFulfilled = await db.Orders
            .AnyAsync(o => o.StripeSessionId == request.StripeSessionId, ct);
 
        if (alreadyFulfilled)
            return;
 
        var package = await db.GigPackages
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.PackageId, ct);
 
        var order = new Order
        {
            Id            = Guid.NewGuid(),
            GigId         = request.GigId,
            PackageId     = request.PackageId,
            BuyerUserId   = request.BuyerUserId,
            StripeSessionId = request.StripeSessionId,
            Status        = OrderStatus.Paid,
            TotalPrice    = request.TotalPrice,
            CreatedAtUtc  = DateTime.UtcNow,
            PaidAtUtc     = request.PaidAtUtc
        };
 
        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);
    }
}