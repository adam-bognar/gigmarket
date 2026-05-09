using FluentValidation;

namespace GigMarket.Application.Features.Messaging.Commands.StartConversation;

public sealed class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
{
    public StartConversationCommandValidator()
    {
        RuleFor(x => x.GigId)
            .NotEmpty().WithMessage("GigId is required.");
    }
}