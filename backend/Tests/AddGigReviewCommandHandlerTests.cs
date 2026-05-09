using FluentAssertions;
using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Reviews.Commands.AddGigReview;
using GigMarket.Domain.Entities;
using GigMarket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Tests;

public class AddGigReviewCommandHandlerTests
{
    private static ApplicationDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_Should_Throw_UnauthorizedException_When_Not_Authenticated()
    {
        await using var db = CreateDbContext();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IsAuthenticated.Returns(false);

        var handler = new AddGigReviewCommandHandler(db, currentUser);
        var command = new AddGigReviewCommand(new AddGigReviewRequest(Guid.NewGuid(), 5, "Great gig overall!"));

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Gig_Does_Not_Exist()
    {
        await using var db = CreateDbContext();
        var currentUser = BuildAuthenticatedUser(Guid.NewGuid());

        var handler = new AddGigReviewCommandHandler(db, currentUser);
        var command = new AddGigReviewCommand(new AddGigReviewRequest(Guid.NewGuid(), 5, "Great gig overall!"));

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Reviewer_Is_The_Seller()
    {
        await using var db = CreateDbContext();
        var sellerUserId = Guid.NewGuid();
        var gig = await SeedGig(db, sellerUserId: sellerUserId);

        var currentUser = BuildAuthenticatedUser(sellerUserId);
        var handler = new AddGigReviewCommandHandler(db, currentUser);
        var command = new AddGigReviewCommand(new AddGigReviewRequest(gig.Id, 5, "Great gig overall!"));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Buyer_Has_No_Completed_Order()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();
        var gig = await SeedGig(db);

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new AddGigReviewCommandHandler(db, currentUser);
        var command = new AddGigReviewCommand(new AddGigReviewRequest(gig.Id, 5, "Great gig overall!"));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Buyer_Already_Reviewed()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();
        var buyerUser = new User { Id = buyerId, UserName = "buyer", CustomUsername = "buyer", Email = "buyer@test.com" };
        db.Users.Add(buyerUser);

        var gig = await SeedGig(db);
        await SeedCompletedOrder(db, gigId: gig.Id, buyerUserId: buyerId);

        var existingReview = new GigReview
        {
            Id = Guid.NewGuid(),
            GigId = gig.Id,
            ReviewerUserId = buyerId,
            Rating = 4,
            Description = "First review already submitted.",
            CreatedAtUtc = DateTime.UtcNow,
            Reviewer = buyerUser
        };
        db.GigReviews.Add(existingReview);
        await db.SaveChangesAsync();

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new AddGigReviewCommandHandler(db, currentUser);
        var command = new AddGigReviewCommand(new AddGigReviewRequest(gig.Id, 5, "Trying to review again!"));

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Create_Review_And_Return_ReviewDto()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();
        var buyerUser = new User { Id = buyerId, UserName = "buyer", CustomUsername = "buyer_user", Email = "buyer@test.com" };
        db.Users.Add(buyerUser);

        var gig = await SeedGig(db);
        await SeedCompletedOrder(db, gigId: gig.Id, buyerUserId: buyerId);
        await db.SaveChangesAsync();

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new AddGigReviewCommandHandler(db, currentUser);
        var command = new AddGigReviewCommand(new AddGigReviewRequest(gig.Id, 5, "Excellent work, very satisfied!"));

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.GigId.Should().Be(gig.Id);
        result.Rating.Should().Be(5);
        result.Description.Should().Be("Excellent work, very satisfied!");
        result.ReviewerUserId.Should().Be(buyerId);
        result.ReviewerUsername.Should().Be("buyer_user");
    }

    [Fact]
    public async Task Handle_Should_Persist_Review_To_Database()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();
        var buyerUser = new User { Id = buyerId, UserName = "buyer", CustomUsername = "buyer_user", Email = "buyer@test.com" };
        db.Users.Add(buyerUser);

        var gig = await SeedGig(db);
        await SeedCompletedOrder(db, gigId: gig.Id, buyerUserId: buyerId);
        await db.SaveChangesAsync();

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new AddGigReviewCommandHandler(db, currentUser);
        var command = new AddGigReviewCommand(new AddGigReviewRequest(gig.Id, 4, "Good work overall, happy with the result!"));

        await handler.Handle(command, CancellationToken.None);

        var saved = await db.GigReviews.SingleAsync(r => r.GigId == gig.Id && r.ReviewerUserId == buyerId);
        saved.Rating.Should().Be(4);
        saved.Description.Should().Be("Good work overall, happy with the result!");
    }

    private static ICurrentUserService BuildAuthenticatedUser(Guid userId)
    {
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns(userId);
        return currentUser;
    }

    private static async Task<Gig> SeedGig(ApplicationDbContext db, Guid? sellerUserId = null)
    {
        var userId = sellerUserId ?? Guid.NewGuid();
        var sellerUser = new User { Id = userId, UserName = "seller", CustomUsername = "seller", Email = "seller@test.com" };
        db.Users.Add(sellerUser);

        var sellerProfile = new SellerProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FirstName = "Seller",
            LastName = "Test",
            Description = "Test seller.",
            ProfileImageUrl = "https://example.com/avatar.jpg"
        };
        db.SellerProfiles.Add(sellerProfile);

        var gig = new Gig
        {
            Id = Guid.NewGuid(),
            SellerProfileId = sellerProfile.Id,
            CategoryId = Guid.NewGuid(),
            SubcategoryId = Guid.NewGuid(),
            Title = "Test Gig",
            Description = "Test description.",
            Status = GigStatus.Active
        };
        db.Gigs.Add(gig);
        await db.SaveChangesAsync();

        return gig;
    }

    private static async Task SeedCompletedOrder(ApplicationDbContext db, Guid gigId, Guid buyerUserId)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            GigId = gigId,
            PackageId = Guid.NewGuid(),
            BuyerUserId = buyerUserId,
            StripeSessionId = Guid.NewGuid().ToString(),
            Status = OrderStatus.Completed,
            TotalPrice = 50m,
            RevisionsUsed = 0,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();
    }
}