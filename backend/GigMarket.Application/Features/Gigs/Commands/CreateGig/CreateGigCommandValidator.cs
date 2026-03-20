using FluentValidation;
using GigMarket.Domain.Entities;

namespace GigMarket.Application.Features.Gigs.Commands.CreateGig;

public sealed class CreateGigCommandValidator : AbstractValidator<CreateGigCommand>
{
    public CreateGigCommandValidator()
    {
        RuleFor(x => x.GigRequest).NotNull().SetValidator(new CreateGigRequestValidator());
    }
}

public sealed class CreateGigRequestValidator : AbstractValidator<CreateGigRequest>
{
    public CreateGigRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(15).MaximumLength(80);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Subcategory).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(120).MaximumLength(1200);

        RuleFor(x => x.Tags).NotEmpty().WithMessage("At least one tag is required.");
        RuleFor(x => x.Tags).Must(t => t.Count <= 5).WithMessage("Maximum 5 tags allowed.");
        RuleForEach(x => x.Tags).NotEmpty().MaximumLength(30);

        RuleFor(x => x.Packages).NotEmpty().WithMessage("At least one package is required.");
        RuleFor(x => x.Packages).Must(p => p.Count <= 3).WithMessage("Maximum 3 packages allowed.");
        RuleForEach(x => x.Packages).SetValidator(new GigPackageRequestValidator());

        RuleForEach(x => x.Requirements).SetValidator(new GigRequirementRequestValidator());

        RuleFor(x => x.PrimaryPhotoUrl).NotEmpty().WithMessage("A primary photo is required.");
        RuleFor(x => x.AdditionalPhotoUrls)
            .Must(p => p == null || p.Count <= 2)
            .WithMessage("Maximum 2 additional photos allowed.");
    }
}

public sealed class GigPackageRequestValidator : AbstractValidator<GigPackageRequest>
{
    public GigPackageRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(20).MaximumLength(120);
        RuleFor(x => x.DeliveryDays).InclusiveBetween(1, 90);
        RuleFor(x => x.Revisions).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(5).WithMessage("Price must be at least $5.");
    }
}

public sealed class GigRequirementRequestValidator : AbstractValidator<GigRequirementRequest>
{
    public GigRequirementRequestValidator()
    {
        RuleFor(x => x.Question).NotEmpty().MinimumLength(5).MaximumLength(300);
        RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);

        RuleFor(x => x.Choices)
            .Must(c => c != null && c.Count >= 2)
            .When(x => x.Type == RequirementType.MultipleChoice)
            .WithMessage("Multiple choice questions require at least 2 options.");

        RuleForEach(x => x.Choices)
            .NotEmpty().MaximumLength(100)
            .When(x => x.Type == RequirementType.MultipleChoice);
    }
}