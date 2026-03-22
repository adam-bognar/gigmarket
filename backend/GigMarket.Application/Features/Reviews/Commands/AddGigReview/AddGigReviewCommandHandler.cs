using AutoMapper;
using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Reviews.Models;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Reviews.Commands.AddGigReview;

public sealed class AddGigReviewCommandHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser,
    IMapper mapper)
    : IRequestHandler<AddGigReviewCommand, ReviewDto>
{
    public async Task<ReviewDto> Handle(AddGigReviewCommand command, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
            throw new UnauthorizedException("Not authenticated.");

        var userId = currentUser.UserId.Value;
        var request = command.Request;

        var gig = await db.Gigs
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == request.GigId, cancellationToken)
            ?? throw new NotFoundException($"Gig with id '{request.GigId}' was not found.");

        var sellerProfile = await db.SellerProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == gig.SellerProfileId, cancellationToken);

        if (sellerProfile?.UserId == userId)
            throw new BadRequestException("You cannot review your own gig.");

        var alreadyReviewed = await db.GigReviews
            .AnyAsync(r => r.GigId == request.GigId && r.ReviewerUserId == userId, cancellationToken);

        if (alreadyReviewed)
            throw new BadRequestException("You have already reviewed this gig.");

        var reviewer = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException("Reviewer user not found.");

        var review = new GigReview
        {
            Id = Guid.NewGuid(),
            GigId = request.GigId,
            ReviewerUserId = userId,
            Rating = request.Rating,
            Description = request.Description,
            CreatedAtUtc = DateTime.UtcNow,
            Reviewer = reviewer
        };

        db.GigReviews.Add(review);
        await db.SaveChangesAsync(cancellationToken);

        return mapper.Map<ReviewDto>(review);
    }
}

