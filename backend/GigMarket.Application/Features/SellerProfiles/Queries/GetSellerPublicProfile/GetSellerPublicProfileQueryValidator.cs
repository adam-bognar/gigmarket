using FluentValidation;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetSellerPublicProfile;

public sealed class GetSellerPublicProfileQueryValidator
    : AbstractValidator<GetSellerPublicProfileQuery>
{
    public GetSellerPublicProfileQueryValidator()
    {
        RuleFor(x => x.SellerProfileId)
            .NotEmpty().WithMessage("Seller profile ID is required.");
    }
}