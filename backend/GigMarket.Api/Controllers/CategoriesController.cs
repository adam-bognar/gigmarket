using GigMarket.Application.Features.Categories.Queries.GetGigCategories;
using GigMarket.Application.Features.Categories.Queries.GetGigSubcategoriesByCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GigMarket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetGigCategoriesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{categoryId:guid}/subcategories")]
    public async Task<IActionResult> GetSubcategories(Guid categoryId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetGigSubcategoriesByCategoryQuery(categoryId), ct);
        return Ok(result);
    }
}

