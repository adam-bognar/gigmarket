using FluentValidation;

namespace GigMarket.Application.Features.Files.Queries.GetFileUrl;

public sealed class GetFileUrlQueryValidator : AbstractValidator<GetFileUrlQuery>
{
    public GetFileUrlQueryValidator()
    {
        RuleFor(x => x.BlobPath).NotEmpty();
    }
}

