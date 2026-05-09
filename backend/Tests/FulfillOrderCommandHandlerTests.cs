using FluentAssertions;
using GigMarket.Application.Features.Orders.Commands.FulfillOrder;
using GigMarket.Domain.Entities;
using GigMarket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class FulfillOrderCommandHandlerTests
{
    private static ApplicationDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_Should_Create_Order_When_Session_Is_New()
    {
        await using var db = CreateDbContext();
        var handler = new FulfillOrderCommandHandler(db);

        var command = CreateValidCommand();
        await handler.Handle(command, CancellationToken.None);

        var order = await db.Orders.SingleAsync(o => o.StripeSessionId == command.StripeSessionId);
        order.Should().NotBeNull();
        order.GigId.Should().Be(command.GigId);
        order.BuyerUserId.Should().Be(command.BuyerUserId);
        order.TotalPrice.Should().Be(command.TotalPrice);
        order.Status.Should().Be(OrderStatus.InProgress);
    }

    [Fact]
    public async Task Handle_Should_Be_Idempotent_When_Session_Already_Fulfilled()
    {
        await using var db = CreateDbContext();
        var handler = new FulfillOrderCommandHandler(db);

        var command = CreateValidCommand();
        await handler.Handle(command, CancellationToken.None);
        await handler.Handle(command, CancellationToken.None);

        var orderCount = await db.Orders.CountAsync(o => o.StripeSessionId == command.StripeSessionId);
        orderCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Should_Set_Deadline_Based_On_Package_DeliveryDays()
    {
        await using var db = CreateDbContext();
        var handler = new FulfillOrderCommandHandler(db);

        var packageId = Guid.NewGuid();
        var package = new GigPackage
        {
            Id = packageId,
            GigId = Guid.NewGuid(),
            Tier = PackageTier.Basic,
            Name = "Basic",
            Description = "Basic package description here.",
            Price = 25,
            DeliveryDays = 10,
            Revisions = 2
        };
        db.GigPackages.Add(package);
        await db.SaveChangesAsync();

        var paidAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var command = CreateValidCommand(packageId: packageId, paidAtUtc: paidAt);

        await handler.Handle(command, CancellationToken.None);

        var order = await db.Orders.SingleAsync(o => o.StripeSessionId == command.StripeSessionId);
        order.DeadlineUtc.Should().Be(paidAt.AddDays(10));
    }

    private static FulfillOrderCommand CreateValidCommand(
        Guid? packageId = null,
        DateTime? paidAtUtc = null) =>
        new(
            StripeSessionId: Guid.NewGuid().ToString(),
            GigId: Guid.NewGuid(),
            PackageId: packageId ?? Guid.NewGuid(),
            BuyerUserId: Guid.NewGuid(),
            TotalPrice: 50m,
            PaidAtUtc: paidAtUtc ?? DateTime.UtcNow);
}