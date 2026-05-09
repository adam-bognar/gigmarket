using FluentValidation.TestHelper;
using GigMarket.Application.Features.Files.Commands.UploadGigMedia;
using GigMarket.Application.Features.Files.Common;

namespace Tests;

public class UploadGigMediaCommandValidatorTests
{
    private readonly UploadGigMediaCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_GigId_Is_Empty()
    {
        var command = CreateValidCommand() with
        {
            GigId = Guid.Empty
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.GigId);
    }

    [Fact]
    public void Should_Have_Error_When_FileStream_Is_Null()
    {
        var command = CreateValidCommand() with
        {
            FileStream = null!
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileStream);
    }

    [Fact]
    public void Should_Have_Error_When_FileName_Is_Empty()
    {
        var command = CreateValidCommand() with
        {
            FileName = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileName);
    }

    [Theory]
    [InlineData("file.txt")]
    [InlineData("file.pdf")]
    [InlineData("file.gif")]
    [InlineData("file.exe")]
    public void Should_Have_Error_When_File_Extension_Is_Not_Supported(string fileName)
    {
        var command = CreateValidCommand() with
        {
            FileName = fileName
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileName);
    }

    [Fact]
    public void Should_Have_Error_When_FileLength_Is_Zero()
    {
        var command = CreateValidCommand() with
        {
            FileLength = 0
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileLength);
    }

    [Fact]
    public void Should_Have_Error_When_FileLength_Is_Negative()
    {
        var command = CreateValidCommand() with
        {
            FileLength = -1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileLength);
    }

    [Fact]
    public void Should_Have_Error_When_Image_File_Is_Larger_Than_Max_Image_Size()
    {
        var command = CreateValidCommand() with
        {
            FileName = "image.jpg",
            ContentType = "image/jpeg",
            FileLength = FileUploadRules.MaxImageSize + 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileLength);
    }

    [Fact]
    public void Should_Have_Error_When_Video_File_Is_Larger_Than_Max_Video_Size()
    {
        var command = CreateValidCommand() with
        {
            FileName = "video.mp4",
            ContentType = "video/mp4",
            FileLength = FileUploadRules.MaxVideoSize + 1
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileLength);
    }

    [Theory]
    [InlineData("image.jpg", "image/jpeg")]
    [InlineData("image.jpeg", "image/jpeg")]
    [InlineData("image.png", "image/png")]
    [InlineData("video.mp4", "video/mp4")]
    public void Should_Not_Have_Error_When_File_Is_Valid(string fileName, string contentType)
    {
        var command = CreateValidCommand() with
        {
            FileName = fileName,
            ContentType = contentType
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static UploadGigMediaCommand CreateValidCommand()
    {
        return new UploadGigMediaCommand(
            GigId: Guid.NewGuid(),
            FileStream: new MemoryStream([1, 2, 3]),
            FileName: "image.jpg",
            ContentType: "image/jpeg",
            FileLength: 1024);
    }
}