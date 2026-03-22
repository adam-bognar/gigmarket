using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Categories.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GigMarket.Application.Features.Categories.Queries.GetGigCategories;

public sealed class GetGigCategoriesQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetGigCategoriesQuery, List<GigCategoryDto>>
{
    public async Task<List<GigCategoryDto>> Handle(GetGigCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await db.GigCategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new GigCategoryDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }
}

