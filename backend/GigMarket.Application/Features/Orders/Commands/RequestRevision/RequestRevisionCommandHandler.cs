using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Orders.Commands.RequestRevision;

public sealed class RequestRevisionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    : IRequestHandler<RequestRevisionCommand>
{
    public async Task Handle(RequestRevisionCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var order = await db.Orders
            .Include(o => o.Package)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct)
            ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

        if (order.BuyerUserId != userId)
            throw new UnauthorizedException("Only the buyer can request a revision.");

        if (order.Status != OrderStatus.Delivered)
            throw new BadRequestException($"Cannot request a revision on an order with status '{order.Status}'.");

        if (order.RevisionsUsed >= order.Package.Revisions)
            throw new BadRequestException("No revisions remaining for this order.");

        var revisionRequest = new OrderRevisionRequest
        {
            Id           = Guid.NewGuid(),
            OrderId      = order.Id,
            Message      = request.Message,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.OrderRevisionRequests.Add(revisionRequest);
        order.RevisionsUsed++;
        order.Status = OrderStatus.UnderRevision;
        await db.SaveChangesAsync(ct);
    }
}