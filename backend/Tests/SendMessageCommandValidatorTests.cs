using FluentValidation.TestHelper;
using GigMarket.Application.Features.Messaging.Commands.SendMessage;

namespace Tests;

public class SendMessageCommandValidatorTests
{
    private readonly SendMessageCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ConversationId_Is_Empty()
    {
        var command = CreateValidCommand() with { ConversationId = Guid.Empty };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ConversationId);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Empty()
    {
        var command = CreateValidCommand() with { Content = "" };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Too_Long()
    {
        var command = CreateValidCommand() with { Content = new string('a', 4001) };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Content);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Command_Is_Valid()
    {
        var result = _validator.TestValidate(CreateValidCommand());

        result.ShouldNotHaveAnyValidationErrors();
    }

    private static SendMessageCommand CreateValidCommand() =>
        new(
            ConversationId: Guid.NewGuid(),
            Content: "Hello, I have a question about your gig.");
}