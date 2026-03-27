using FluentValidation;

namespace GigMarket.Application.Features.Gigs.Commands.DeleteGig;

public sealed class DeleteGigCommandValidator : AbstractValidator<DeleteGigCommand>
{
    public DeleteGigCommandValidator()
    {
        RuleFor(x => x.GigId).NotEmpty();
    }
}

