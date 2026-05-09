using FluentAssertions;
using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Orders.Commands.AcceptDelivery;
using GigMarket.Domain.Entities;
using GigMarket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Tests;

public class AcceptDeliveryCommandHandlerTests
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

        var handler = new AcceptDeliveryCommandHandler(db, currentUser, Substitute.For<ILogger<AcceptDeliveryCommandHandler>>());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new AcceptDeliveryCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Order_Does_Not_Exist()
    {
        await using var db = CreateDbContext();
        var currentUser = BuildAuthenticatedUser(Guid.NewGuid());

        var handler = new AcceptDeliveryCommandHandler(db, currentUser, Substitute.For<ILogger<AcceptDeliveryCommandHandler>>());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new AcceptDeliveryCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_UnauthorizedException_When_User_Is_Not_The_Buyer()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        var order = await SeedDeliveredOrder(db, buyerUserId: buyerId);

        var currentUser = BuildAuthenticatedUser(otherId);
        var handler = new AcceptDeliveryCommandHandler(db, currentUser, Substitute.For<ILogger<AcceptDeliveryCommandHandler>>());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new AcceptDeliveryCommand(order.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Order_Status_Is_Not_Delivered()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();

        var order = await SeedDeliveredOrder(db, buyerUserId: buyerId, status: OrderStatus.InProgress);

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new AcceptDeliveryCommandHandler(db, currentUser, Substitute.For<ILogger<AcceptDeliveryCommandHandler>>());

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.Handle(new AcceptDeliveryCommand(order.Id), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Set_Order_Status_To_Completed()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();

        var order = await SeedDeliveredOrder(db, buyerUserId: buyerId);

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new AcceptDeliveryCommandHandler(db, currentUser, Substitute.For<ILogger<AcceptDeliveryCommandHandler>>());

        await handler.Handle(new AcceptDeliveryCommand(order.Id), CancellationToken.None);

        var updated = await db.Orders.FindAsync(order.Id);
        updated!.Status.Should().Be(OrderStatus.Completed);
    }

    private static ICurrentUserService BuildAuthenticatedUser(Guid userId)
    {
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns(userId);
        return currentUser;
    }
    private static async Task<Order> SeedDeliveredOrder(
        ApplicationDbContext db,
        Guid buyerUserId,
        OrderStatus status = OrderStatus.Delivered)
    {
        var sellerUserId = Guid.NewGuid();
        var sellerUser = new User { Id = sellerUserId, UserName = "seller", CustomUsername = "seller", Email = "seller@test.com" };
        db.Users.Add(sellerUser);

        var sellerProfile = new SellerProfile
        {
            Id = Guid.NewGuid(),
            UserId = sellerUserId,
            FirstName = "Seller",
            LastName = "Test",
            Description = "A test seller.",
            ProfileImageUrl = "https://example.com/avatar.jpg",
            StripeAccountId = null,
            StripeAccountStatus = SellerStripeAccountStatus.NotConnected
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

        var order = new Order
        {
            Id = Guid.NewGuid(),
            GigId = gig.Id,
            PackageId = Guid.NewGuid(),
            BuyerUserId = buyerUserId,
            StripeSessionId = Guid.NewGuid().ToString(),
            Status = status,
            TotalPrice = 100m,
            RevisionsUsed = 0,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync();

        return order;
    }
}