using FluentValidation;

namespace GigMarket.Application.Features.Messaging.Commands.SendMessage;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty().WithMessage("ConversationId is required.");
 
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(4000).WithMessage("Message cannot exceed 4000 characters.");
    }
}