using FluentValidation;

namespace GigMarket.Application.Features.Reviews.Commands.AddGigReview;

public sealed class AddGigReviewCommandValidator : AbstractValidator<AddGigReviewCommand>
{
    public AddGigReviewCommandValidator()
    {
        RuleFor(x => x.Request).NotNull().SetValidator(new AddGigReviewRequestValidator());
    }
}

public sealed class AddGigReviewRequestValidator : AbstractValidator<AddGigReviewRequest>
{
    public AddGigReviewRequestValidator()
    {
        RuleFor(x => x.GigId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5 stars.");
        RuleFor(x => x.Description).NotEmpty()
            .MinimumLength(10).WithMessage("Review must be at least 10 characters.")
            .MaximumLength(2000).WithMessage("Review cannot exceed 2000 characters.");
    }
}

