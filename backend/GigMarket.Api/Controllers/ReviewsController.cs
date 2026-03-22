using GigMarket.Application.Features.Reviews.Commands.AddGigReview;
using GigMarket.Application.Features.Reviews.Models;
using GigMarket.Application.Features.Reviews.Queries.GetGigReviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GigMarket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReviewDto>> AddReview(
        [FromBody] AddGigReviewRequest request,
        CancellationToken ct)
    {
        var result = await mediator.Send(new AddGigReviewCommand(request), ct);
        return Ok(result);
    }
    
    [HttpGet("{gigId:guid}")]
    [ProducesResponseType(typeof(GetGigReviewsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetGigReviewsResult>> GetReviews(
        Guid gigId,
        CancellationToken ct)
    {
        var result = await mediator.Send(new GetGigReviewsQuery(gigId), ct);
        return Ok(result);
    }
}

