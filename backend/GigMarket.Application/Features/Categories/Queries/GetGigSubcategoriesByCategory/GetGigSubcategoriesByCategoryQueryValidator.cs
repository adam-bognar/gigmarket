using FluentValidation;

namespace GigMarket.Application.Features.Categories.Queries.GetGigSubcategoriesByCategory;

public sealed class GetGigSubcategoriesByCategoryQueryValidator : AbstractValidator<GetGigSubcategoriesByCategoryQuery>
{
    public GetGigSubcategoriesByCategoryQueryValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}

