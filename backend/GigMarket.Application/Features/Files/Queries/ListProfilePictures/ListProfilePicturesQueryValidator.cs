using FluentValidation;

namespace GigMarket.Application.Features.Files.Queries.ListProfilePictures;

public sealed class ListProfilePicturesQueryValidator : AbstractValidator<ListProfilePicturesQuery>
{
    public ListProfilePicturesQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

