using FluentValidation;

namespace GigMarket.Application.Features.Messaging.Commands.StartConversation;

public sealed class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
{
    public StartConversationCommandValidator()
    {
        RuleFor(x => x.GigId)
            .NotEmpty().WithMessage("GigId is required.");
 
        RuleFor(x => x.InitialMessage)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(4000).WithMessage("Message cannot exceed 4000 characters.");
    }
}