using FluentValidation;

namespace GigMarket.Application.Features.Reviews.Queries.GetGigReviews;

public sealed class GetGigReviewsQueryValidator : AbstractValidator<GetGigReviewsQuery>
{
    public GetGigReviewsQueryValidator()
    {
        RuleFor(x => x.GigId).NotEmpty();
    }
}

