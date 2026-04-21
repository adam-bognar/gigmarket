using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Files.Commands.UploadOrderDeliveryFile;

public sealed class UploadOrderDeliveryFileCommandHandler(
    IBlobStorageService blobStorage,
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : IRequestHandler<UploadOrderDeliveryFileCommand, UploadOrderDeliveryFileResult>
{
    public async Task<UploadOrderDeliveryFileResult> Handle(
        UploadOrderDeliveryFileCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId!.Value;

        var order = await db.Orders
                        .AsNoTracking()
                        .Include(o => o.Gig)
                        .ThenInclude(g => g.SellerProfile)
                        .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct)
                    ?? throw new NotFoundException($"Order '{request.OrderId}' not found.");

        if (order.Gig.SellerProfile.UserId != userId)
            throw new UnauthorizedException("Only the seller can upload delivery files.");

        var folder = $"orders/{request.OrderId}/deliveries";

        var url = await blobStorage.UploadFileAsync(
            request.FileStream,
            request.FileName,
            request.ContentType,
            folder,
            ct);

        return new UploadOrderDeliveryFileResult(url);
    }
}