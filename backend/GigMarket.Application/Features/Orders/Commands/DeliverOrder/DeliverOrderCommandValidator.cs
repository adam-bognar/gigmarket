using FluentValidation;

namespace GigMarket.Application.Features.Orders.Commands.DeliverOrder;

public sealed class DeliverOrderCommandValidator : AbstractValidator<DeliverOrderCommand>
{
    public DeliverOrderCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Message).NotEmpty().MaximumLength(4000);
        RuleFor(x => x.FileUrls).NotNull();
        RuleForEach(x => x.FileUrls).NotEmpty().MaximumLength(2048);
    }
}