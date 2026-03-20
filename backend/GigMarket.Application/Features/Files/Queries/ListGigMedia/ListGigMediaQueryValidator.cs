using FluentValidation;

namespace GigMarket.Application.Features.Files.Queries.ListGigMedia;

public sealed class ListGigMediaQueryValidator : AbstractValidator<ListGigMediaQuery>
{
    public ListGigMediaQueryValidator()
    {
        RuleFor(x => x.GigId).NotEmpty();
    }
}

