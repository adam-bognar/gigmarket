using FluentValidation.TestHelper;
using GigMarket.Application.Features.Files.Commands.DeleteFile;

namespace Tests;

public class DeleteFileCommandValidatorTests
{
    private readonly DeleteFileCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_BlobPath_Is_Empty()
    {
        var command = CreateValidCommand() with
        {
            BlobPath = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BlobPath);
    }

    [Fact]
    public void Should_Have_Error_When_BlobPath_Is_WhiteSpace()
    {
        var command = CreateValidCommand() with
        {
            BlobPath = "   "
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BlobPath);
    }

    [Fact]
    public void Should_Not_Have_Error_When_BlobPath_Is_Provided()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static DeleteFileCommand CreateValidCommand()
    {
        return new DeleteFileCommand(
            BlobPath: "gig-media/seller-1/gig-1/photo.jpg");
    }
}