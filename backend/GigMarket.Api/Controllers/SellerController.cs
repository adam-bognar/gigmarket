using GigMarket.Application.Features.SellerProfiles.Commands.CreateSellerProfile;
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
    public async Task<ActionResult<bool>> Create([FromBody] CreateSellerProfileRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateSellerProfileCommand(request), ct);
        return Ok(result);
    }
    
}