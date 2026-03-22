using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Categories.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Categories.Queries.GetGigSubcategoriesByCategory;

public sealed class GetGigSubcategoriesByCategoryQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetGigSubcategoriesByCategoryQuery, List<GigSubcategoryDto>>
{
    public async Task<List<GigSubcategoryDto>> Handle(
        GetGigSubcategoriesByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        return await db.GigSubcategories
            .AsNoTracking()
            .Where(x => x.CategoryId == request.CategoryId)
            .OrderBy(x => x.Name)
            .Select(x => new GigSubcategoryDto(x.Id, x.CategoryId, x.Name))
            .ToListAsync(cancellationToken);
    }
}

