using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Orders.Commands.AcceptDelivery;
using GigMarket.Application.Features.Orders.Commands.CreateCheckoutSession;
using GigMarket.Application.Features.Orders.Commands.DeliverOrder;
using GigMarket.Application.Features.Orders.Commands.FulfillOrder;
using GigMarket.Application.Features.Orders.Commands.RequestRevision;
using GigMarket.Application.Features.Orders.Models;
using GigMarket.Application.Features.Orders.Queries.GetMyOrders;
using GigMarket.Application.Features.Orders.Queries.GetMySellerOrders;
using GigMarket.Application.Features.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace GigMarket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(
    IMediator mediator,
    IConfiguration configuration,
    ILogger<OrdersController> logger,
    IStripeWebhookService stripeWebhookService) : ControllerBase
{
    [HttpPost("checkout")]
    [Authorize]
    [ProducesResponseType(typeof(CheckoutResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request, CancellationToken ct)
    {
        var sessionUrl = await mediator.Send(
            new CreateCheckoutSessionCommand(request.GigId, request.PackageId), ct);

        return Ok(new CheckoutResponseDto(sessionUrl));
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyOrdersQuery(), ct);
        return Ok(result);
    }

    [HttpGet("seller")]
    [Authorize]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMySellerOrders(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMySellerOrdersQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetOrderByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/deliver")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Deliver(Guid id, [FromBody] DeliverRequest request, CancellationToken ct)
    {
        await mediator.Send(new DeliverOrderCommand(id, request.Message, request.FileUrls), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/request-revision")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RequestRevision(Guid id, [FromBody] RevisionRequest request, CancellationToken ct)
    {
        await mediator.Send(new RequestRevisionCommand(id, request.Message), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/accept")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Accept(Guid id, CancellationToken ct)
    {
        await mediator.Send(new AcceptDeliveryCommand(id), ct);
        return NoContent();
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        var webhookSecret = configuration["Stripe:WebhookSecret"]
                            ?? throw new InvalidOperationException("Stripe:WebhookSecret is not configured.");

        Request.Body.Position = 0;
        string json;
        using (var reader = new StreamReader(HttpContext.Request.Body))
            json = await reader.ReadToEndAsync(ct);

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                webhookSecret,
                throwOnApiVersionMismatch: false);
        }
        catch (StripeException ex)
        {
            return BadRequest(new { error = ex.Message });
        }

        var result = await stripeWebhookService.HandleAsync(stripeEvent, ct);

        if (!result.Success)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok();
    }
}

public sealed record CheckoutRequest(Guid GigId, Guid PackageId);
public sealed record CheckoutResponseDto(string SessionUrl);
public sealed record DeliverRequest(string Message, List<string> FileUrls);
public sealed record RevisionRequest(string Message);