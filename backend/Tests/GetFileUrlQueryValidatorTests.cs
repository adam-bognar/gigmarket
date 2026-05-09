using FluentValidation.TestHelper;
using GigMarket.Application.Features.Files.Queries.GetFileUrl;

namespace Tests;

public class GetFileUrlQueryValidatorTests
{
    private readonly GetFileUrlQueryValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_BlobPath_Is_Empty()
    {
        var query = CreateValidQuery() with
        {
            BlobPath = ""
        };

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.BlobPath);
    }

    [Fact]
    public void Should_Have_Error_When_BlobPath_Is_WhiteSpace()
    {
        var query = CreateValidQuery() with
        {
            BlobPath = "   "
        };

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.BlobPath);
    }

    [Fact]
    public void Should_Not_Have_Error_When_BlobPath_Is_Provided()
    {
        var query = CreateValidQuery();

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static GetFileUrlQuery CreateValidQuery()
    {
        return new GetFileUrlQuery(
            BlobPath: "gig-media/seller-1/gig-1/photo.jpg");
    }
}