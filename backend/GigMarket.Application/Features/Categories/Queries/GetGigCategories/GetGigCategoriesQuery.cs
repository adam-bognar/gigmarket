using GigMarket.Application.Features.Categories.Models;
using MediatR;

namespace GigMarket.Application.Features.Categories.Queries.GetGigCategories;

public sealed record GetGigCategoriesQuery : IRequest<List<GigCategoryDto>>;

