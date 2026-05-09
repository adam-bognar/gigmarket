using FluentValidation.TestHelper;
using GigMarket.Application.Features.Orders.Commands.RequestRevision;

namespace Tests;

public class RequestRevisionCommandValidatorTests
{
    private readonly RequestRevisionCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_OrderId_Is_Empty()
    {
        var command = CreateValidCommand() with { OrderId = Guid.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.OrderId);
    }

    [Fact]
    public void Should_Have_Error_When_Message_Is_Empty()
    {
        var command = CreateValidCommand() with { Message = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Have_Error_When_Message_Is_Too_Long()
    {
        var command = CreateValidCommand() with { Message = new string('a', 4001) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Message);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var result = _validator.TestValidate(CreateValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static RequestRevisionCommand CreateValidCommand() =>
        new( Guid.NewGuid(),
            "Please adjust the color scheme to match the brand guidelines.");
}