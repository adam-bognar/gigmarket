using FluentValidation.TestHelper;
using GigMarket.Application.Features.Auth.Commands.Login;

namespace Tests;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();
    
    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        var command = CreateValidCommand() with
        {
            Email = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
    
    [Theory]
    [InlineData("not-an-email")]
    [InlineData("adam@")]
    [InlineData("@example.com")]
    [InlineData("adam.example.com")]
    public void Should_Have_Error_When_Email_Is_Invalid(string email)
    {
        var command = CreateValidCommand() with
        {
            Email = email
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
    
    [Fact]
    public void Should_Have_Error_When_Email_Is_Too_Long()
    {
        var longEmail = $"{new string('a', 245)}@example.com";

        var command = CreateValidCommand() with
        {
            Email = longEmail
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }
    
    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        var command = CreateValidCommand() with
        {
            Password = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
    
    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Long()
    {
        var command = CreateValidCommand() with
        {
            Password = new string('a', 101)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
    
    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var command = CreateValidCommand();

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
    
    private static LoginCommand CreateValidCommand()
    {
        return new LoginCommand(
            Email: "adam@example.com",
            Password: "Password1!",
            RememberMe: false);
    }
}