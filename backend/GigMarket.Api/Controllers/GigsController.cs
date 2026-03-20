using GigMarket.Application.Features.Gigs.Commands.CreateGig;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GigMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GigsController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGigRequest request, CancellationToken ct)
        {
            var result = await mediator.Send(new CreateGigCommand(request), ct);
            return Ok(result);
        }
    }
}
