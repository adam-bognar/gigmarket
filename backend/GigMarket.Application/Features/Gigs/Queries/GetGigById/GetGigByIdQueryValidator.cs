using FluentValidation;

namespace GigMarket.Application.Features.Gigs.Queries.GetGigById;

public sealed class GetGigByIdQueryValidator : AbstractValidator<GetGigByIdQuery>
{
    public GetGigByIdQueryValidator()
    {
        RuleFor(x => x.GigId).NotEmpty();
    }
}

