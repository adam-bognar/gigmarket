using FluentValidation.TestHelper;
using GigMarket.Application.Features.Orders.Commands.DeliverOrder;

namespace Tests;

public class DeliverOrderCommandValidatorTests
{
     private readonly DeliverOrderCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_OrderId_Is_Empty()
    {
        var command = CreateValidCommand() with
        {
            OrderId = Guid.Empty
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public void Should_Have_Error_When_Message_Is_Empty()
    {
        var command = CreateValidCommand() with
        {
            Message = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Have_Error_When_Message_Is_Too_Long()
    {
        var command = CreateValidCommand() with
        {
            Message = new string('a', 4001)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Have_Error_When_FileUrls_Is_Null()
    {
        var command = CreateValidCommand() with
        {
            FileUrls = null!
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FileUrls);
    }

    [Fact]
    public void Should_Have_Error_When_FileUrl_Is_Empty()
    {
        var command = CreateValidCommand() with
        {
            FileUrls = ["https://example.com/file-1.zip", ""]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("FileUrls[1]");
    }

    [Fact]
    public void Should_Have_Error_When_FileUrl_Is_Too_Long()
    {
        var command = CreateValidCommand() with
        {
            FileUrls =
            [
                "https://example.com/file-1.zip",
                new string('a', 2049)
            ]
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor("FileUrls[1]");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static DeliverOrderCommand CreateValidCommand()
    {
        return new DeliverOrderCommand(
            OrderId: Guid.NewGuid(),
            Message: "The order has been completed. Please find the delivered files attached.",
            FileUrls:
            [
                "https://example.com/delivery/file-1.zip",
                "https://example.com/delivery/file-2.pdf"
            ]);
    }
}