using FluentAssertions;
using GigMarket.Application.Features.Orders.Commands.FulfillOrder;
using GigMarket.Application.Features.Orders.Services;
using GigMarket.Infrastructure.Service;
using MediatR;
using NSubstitute;
using Stripe;
using Stripe.Checkout;

namespace Tests;

public class StripeWebhookServiceTests
{
    [Fact]
    public async Task HandleAsync_Should_Send_FulfillOrderCommand_When_CheckoutSessionCompleted_Is_Valid()
    {
        var mediator = Substitute.For<IMediator>();
        var service = new StripeWebhookService(mediator);

        var gigId = Guid.NewGuid();
        var packageId = Guid.NewGuid();
        var buyerUserId = Guid.NewGuid();

        var paidAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var stripeEvent = CreateCheckoutSessionCompletedEvent(
            sessionId: "cs_test_123",
            gigId: gigId,
            packageId: packageId,
            buyerUserId: buyerUserId,
            amountTotal: 2500,
            created: paidAt);

        var result = await service.HandleAsync(stripeEvent, CancellationToken.None);

        result.Success.Should().BeTrue();

        await mediator.Received(1).Send(
            Arg.Is<FulfillOrderCommand>(command =>
                command!.StripeSessionId == "cs_test_123" &&
                command.GigId == gigId &&
                command.PackageId == packageId &&
                command.BuyerUserId == buyerUserId &&
                command.TotalPrice == 25m &&
                command.PaidAtUtc == paidAt),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Return_BadRequest_When_Metadata_Is_Missing()
    {
        var mediator = Substitute.For<IMediator>();
        var service = new StripeWebhookService(mediator);

        var stripeEvent = new Event
        {
            Type = EventTypes.CheckoutSessionCompleted,
            Data = new EventData
            {
                Object = new Session
                {
                    Id = "cs_test_123",
                    AmountTotal = 2500,
                    Created = DateTime.UtcNow,
                    Metadata = new Dictionary<string, string>()
                }
            }
        };

        var result = await service.HandleAsync(stripeEvent, CancellationToken.None);

        result.Success.Should().BeFalse();
        result.Error.Should().Be("Missing or invalid metadata on Stripe session.");

        await mediator.DidNotReceive().Send(
            Arg.Any<IRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_Ignore_Non_CheckoutSessionCompleted_Events()
    {
        var mediator = Substitute.For<IMediator>();
        var service = new StripeWebhookService(mediator);

        var stripeEvent = new Event
        {
            Type = EventTypes.PaymentIntentSucceeded,
            Data = new EventData()
        };

        var result = await service.HandleAsync(stripeEvent, CancellationToken.None);

        result.Success.Should().BeTrue();

        await mediator.DidNotReceive().Send(
            Arg.Any<IRequest>(),
            Arg.Any<CancellationToken>());
    }

    private static Event CreateCheckoutSessionCompletedEvent(
        string sessionId,
        Guid gigId,
        Guid packageId,
        Guid buyerUserId,
        long amountTotal,
        DateTime created)
    {
        return new Event
        {
            Type = EventTypes.CheckoutSessionCompleted,
            Data = new EventData
            {
                Object = new Session
                {
                    Id = sessionId,
                    AmountTotal = amountTotal,
                    Created = created,
                    Metadata = new Dictionary<string, string>
                    {
                        ["gigId"] = gigId.ToString(),
                        ["packageId"] = packageId.ToString(),
                        ["buyerUserId"] = buyerUserId.ToString()
                    }
                }
            }
        };
    }
}