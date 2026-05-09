using FluentValidation.TestHelper;
using GigMarket.Application.Features.Orders.Commands.CreateCheckoutSession;

namespace Tests;

public class CreateCheckoutSessionCommandValidatorTests
{
    private readonly CreateCheckoutSessionCommandValidator _validator = new();

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
    public void Should_Have_Error_When_PackageId_Is_Empty()
    {
        var command = CreateValidCommand() with
        {
            PackageId = Guid.Empty
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.PackageId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static CreateCheckoutSessionCommand CreateValidCommand()
    {
        return new CreateCheckoutSessionCommand(
            GigId: Guid.NewGuid(),
            PackageId: Guid.NewGuid());
    }
}