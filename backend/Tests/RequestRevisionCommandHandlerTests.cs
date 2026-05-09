using FluentAssertions;
using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Orders.Commands.RequestRevision;
using GigMarket.Domain.Entities;
using GigMarket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Tests;

public class RequestRevisionCommandHandlerTests
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

        var handler = new RequestRevisionCommandHandler(db, currentUser);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new RequestRevisionCommand(Guid.NewGuid(), "Please revise."), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Order_Does_Not_Exist()
    {
        await using var db = CreateDbContext();
        var currentUser = BuildAuthenticatedUser(Guid.NewGuid());

        var handler = new RequestRevisionCommandHandler(db, currentUser);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new RequestRevisionCommand(Guid.NewGuid(), "Please revise."), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_UnauthorizedException_When_User_Is_Not_The_Buyer()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        var order = SeedDeliveredOrder(db, buyerUserId: buyerId, revisions: 2);

        var currentUser = BuildAuthenticatedUser(otherId);
        var handler = new RequestRevisionCommandHandler(db, currentUser);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(new RequestRevisionCommand(order.Id, "Please revise."), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Order_Status_Is_Not_Delivered()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();

        var order = SeedDeliveredOrder(db, buyerUserId: buyerId, revisions: 2, status: OrderStatus.InProgress);

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new RequestRevisionCommandHandler(db, currentUser);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.Handle(new RequestRevisionCommand(order.Id, "Please revise."), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_No_Revisions_Remaining()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();

        var order = SeedDeliveredOrder(db, buyerUserId: buyerId, revisions: 1, revisionsUsed: 1);

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new RequestRevisionCommandHandler(db, currentUser);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.Handle(new RequestRevisionCommand(order.Id, "Please revise."), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Set_Order_Status_To_UnderRevision()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();

        var order = SeedDeliveredOrder(db, buyerUserId: buyerId, revisions: 2);

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new RequestRevisionCommandHandler(db, currentUser);

        await handler.Handle(new RequestRevisionCommand(order.Id, "Please revise the colors."), CancellationToken.None);

        var updated = await db.Orders.FindAsync(order.Id);
        updated!.Status.Should().Be(OrderStatus.UnderRevision);
    }

    [Fact]
    public async Task Handle_Should_Increment_RevisionsUsed()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();

        var order = SeedDeliveredOrder(db, buyerUserId: buyerId, revisions: 3, revisionsUsed: 1);

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new RequestRevisionCommandHandler(db, currentUser);

        await handler.Handle(new RequestRevisionCommand(order.Id, "Please revise the layout."), CancellationToken.None);

        var updated = await db.Orders.FindAsync(order.Id);
        updated!.RevisionsUsed.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Should_Create_OrderRevisionRequest_Record()
    {
        await using var db = CreateDbContext();
        var buyerId = Guid.NewGuid();
        const string message = "Please revise the fonts.";

        var order = SeedDeliveredOrder(db, buyerUserId: buyerId, revisions: 2);

        var currentUser = BuildAuthenticatedUser(buyerId);
        var handler = new RequestRevisionCommandHandler(db, currentUser);

        await handler.Handle(new RequestRevisionCommand(order.Id, message), CancellationToken.None);

        var revisionRequest = await db.OrderRevisionRequests.SingleAsync(r => r.OrderId == order.Id);
        revisionRequest.Message.Should().Be(message);
    }

    private static ICurrentUserService BuildAuthenticatedUser(Guid userId)
    {
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns(userId);
        return currentUser;
    }

    private static Order SeedDeliveredOrder(
        ApplicationDbContext db,
        Guid buyerUserId,
        int revisions,
        int revisionsUsed = 0,
        OrderStatus status = OrderStatus.Delivered)
    {
        var packageId = Guid.NewGuid();
        var package = new GigPackage
        {
            Id = packageId,
            GigId = Guid.NewGuid(),
            Tier = PackageTier.Basic,
            Name = "Basic",
            Description = "Basic package description here.",
            Price = 25,
            DeliveryDays = 7,
            Revisions = revisions
        };
        db.GigPackages.Add(package);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            GigId = Guid.NewGuid(),
            PackageId = packageId,
            BuyerUserId = buyerUserId,
            StripeSessionId = Guid.NewGuid().ToString(),
            Status = status,
            TotalPrice = 50m,
            RevisionsUsed = revisionsUsed,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Orders.Add(order);
        db.SaveChanges();

        return order;
    }
}