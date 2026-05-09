using GigMarket.Application.Features.SellerProfiles.Commands.ConnectStripeAccount;
using GigMarket.Application.Features.SellerProfiles.Commands.CreateSellerProfile;
using GigMarket.Application.Features.SellerProfiles.Commands.UpdateSellerProfile;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Application.Features.SellerProfiles.Queries.GetMySellerProfile;
using GigMarket.Application.Features.SellerProfiles.Queries.GetSellerEarnings;
using GigMarket.Application.Features.SellerProfiles.Queries.GetSellerPublicProfile;
using GigMarket.Application.Features.SellerProfiles.Queries.GetStripeDashboardLink;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GigMarket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SellerController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateSellerProfileRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateSellerProfileCommand(request), ct);
        return Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMySellerProfileQuery(), ct);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateSellerProfileRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateSellerProfileCommand(request), ct);
        return NoContent();
    }

    [HttpGet("{id}/public")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicProfile(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSellerPublicProfileQuery(id), ct);
        return Ok(result);
    }

    [HttpPost("connect")]
    [ProducesResponseType(typeof(ConnectStripeAccountResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConnectStripe(CancellationToken ct)
    {
        var result = await mediator.Send(new ConnectStripeAccountCommand(), ct);
        return Ok(result);
    }

    [HttpGet("connect/dashboard")]
    [ProducesResponseType(typeof(StripeDashboardLinkDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> StripeDashboard(CancellationToken ct)
    {
        var url = await mediator.Send(new GetStripeDashboardLinkQuery(), ct);
        return Ok(new StripeDashboardLinkDto(url));
    }

    [HttpGet("earnings")]
    [ProducesResponseType(typeof(SellerEarningsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEarnings(CancellationToken ct)
    {
        var result = await mediator.Send(new GetSellerEarningsQuery(), ct);
        return Ok(result);
    }
}

public sealed record StripeDashboardLinkDto(string Url);