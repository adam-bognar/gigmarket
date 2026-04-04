using FluentValidation;

namespace GigMarket.Application.Features.Orders.Commands.CreateCheckoutSession;

public sealed class CreateCheckoutSessionCommandValidator : AbstractValidator<CreateCheckoutSessionCommand>
{
    public CreateCheckoutSessionCommandValidator()
    {
        RuleFor(x => x.GigId).NotEmpty();
        RuleFor(x => x.PackageId).NotEmpty();
    }
}