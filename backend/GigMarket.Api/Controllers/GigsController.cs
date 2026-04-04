using GigMarket.Application.Features.Gigs.Commands.CreateGig;
using GigMarket.Application.Features.Gigs.Commands.CreateGigDraft;
using GigMarket.Application.Features.Gigs.Commands.DeleteGig;
using GigMarket.Application.Features.Gigs.Commands.UpdateGig;
using GigMarket.Application.Features.Gigs.Models;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Application.Features.Gigs.Queries.GetGigById;
using GigMarket.Application.Features.Gigs.Queries.GetGigs;
using GigMarket.Application.Features.Gigs.Queries.GetMyGigs;
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
        public async Task<ActionResult<List<GigSummaryDto>>> GetAll(
            [FromQuery] string? search,
            [FromQuery] Guid? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? deliveryTime,
            [FromQuery] double? minRating,
            CancellationToken ct)
        {
            var result = await mediator.Send(new GetGigsQuery(search, categoryId, minPrice, maxPrice, deliveryTime, minRating), ct);
            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(List<GigSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GigSummaryDto>>> GetMyGigs(CancellationToken ct)
        {
            var result = await mediator.Send(new GetMyGigsQuery(), ct);
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
        
        [HttpPost("draft")]
        public async Task<ActionResult<GigDto>> CreateDraft(CancellationToken ct)
        {
            var result = await mediator.Send(new CreateGigDraftCommand(), ct);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(GigDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGigRequest request, CancellationToken ct)
        {
            var result = await mediator.Send(new UpdateGigCommand(id, request), ct);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await mediator.Send(new DeleteGigCommand(id), ct);
            return NoContent();
        }
    }
}
