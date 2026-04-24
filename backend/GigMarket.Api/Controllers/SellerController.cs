using GigMarket.Application.Features.SellerProfiles.Commands.CreateSellerProfile;
using GigMarket.Application.Features.SellerProfiles.Commands.UpdateSellerProfile;
using GigMarket.Application.Features.SellerProfiles.Queries.GetMySellerProfile;
using GigMarket.Application.Features.SellerProfiles.Queries.GetSellerPublicProfile;
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
}