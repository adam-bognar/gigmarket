using GigMarket.Application.Features.Categories.Models;
using MediatR;

namespace GigMarket.Application.Features.Categories.Queries.GetGigSubcategoriesByCategory;

public sealed record GetGigSubcategoriesByCategoryQuery(Guid CategoryId)
    : IRequest<List<GigSubcategoryDto>>;

