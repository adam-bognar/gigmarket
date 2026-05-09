using FluentValidation.TestHelper;
using GigMarket.Application.Features.Auth.Commands.Register;

namespace Tests;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _validator = new();
    
    [Fact]
    public void Should_Have_Error_When_Username_Is_Empty()
    {
        var command = CreateValidCommand() with
        {
            CustomUsername = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomUsername);
    }
    
    [Fact]
    public void Should_Have_Error_When_Username_Is_Too_Short()
    {
        var command = CreateValidCommand() with
        {
            CustomUsername = "ab"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomUsername);
    }
    
    [Fact]
    public void Should_Have_Error_When_Username_Is_Too_Long()
    {
        var command = CreateValidCommand() with
        {
            CustomUsername = new string('a', 31)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomUsername);
    }
    
    [Theory]
    [InlineData("adam-bognar")]
    [InlineData("adam bognar")]
    [InlineData("adam.bognar")]
    [InlineData("adam@bognar")]
    public void Should_Have_Error_When_Username_Contains_Invalid_Characters(string username)
    {
        var command = CreateValidCommand() with
        {
            CustomUsername = username
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CustomUsername);
    }
    
    [Fact]
    public void Should_Not_Have_Error_When_Username_Contains_Letters_Digits_And_Underscore()
    {
        var command = CreateValidCommand() with
        {
            CustomUsername = "adam_123"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.CustomUsername);
    }
    
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
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        var command = CreateValidCommand() with
        {
            Password = "Aa1!"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
    
    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Long()
    {
        var command = CreateValidCommand() with
        {
            Password = $"Aa1!{new string('a', 97)}"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
    
    [Fact]
    public void Should_Have_Error_When_Password_Does_Not_Contain_Uppercase_Letter()
    {
        var command = CreateValidCommand() with
        {
            Password = "password1!"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
    
    [Fact]
    public void Should_Have_Error_When_Password_Does_Not_Contain_Lowercase_Letter()
    {
        var command = CreateValidCommand() with
        {
            Password = "PASSWORD1!"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
    
    [Fact]
    public void Should_Have_Error_When_Password_Does_Not_Contain_Digit()
    {
        var command = CreateValidCommand() with
        {
            Password = "Password!"
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
    
    [Fact]
    public void Should_Have_Error_When_Password_Does_Not_Contain_Special_Character()
    {
        var command = CreateValidCommand() with
        {
            Password = "Password1"
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
    
    private static RegisterCommand CreateValidCommand()
    {
        return new RegisterCommand(
            CustomUsername: "adam_123",
            Email: "adam@example.com",
            Password: "Password1!");
    }
}