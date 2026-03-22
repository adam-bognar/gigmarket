using GigMarket.Application.Features.Gigs.Commands.CreateGig;
using GigMarket.Application.Features.Gigs.Models;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Application.Features.Gigs.Queries.GetGigById;
using GigMarket.Application.Features.Gigs.Queries.GetGigs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GigMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GigsController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<GigSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GigSummaryDto>>> GetAll(CancellationToken ct)
        {
            var result = await mediator.Send(new GetGigsQuery(), ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GigDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GigDetailDto>> GetById(Guid id, CancellationToken ct)
        {
            var result = await mediator.Send(new GetGigByIdQuery(id), ct);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(GigDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateGigRequest request, CancellationToken ct)
        {
            var result = await mediator.Send(new CreateGigCommand(request), ct);
            return Ok(result);
        }
    }
}
