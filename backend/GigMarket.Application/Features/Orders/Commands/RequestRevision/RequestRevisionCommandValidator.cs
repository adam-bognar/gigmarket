using FluentValidation;

namespace GigMarket.Application.Features.Orders.Commands.RequestRevision;

public sealed class RequestRevisionCommandValidator : AbstractValidator<RequestRevisionCommand>
{
    public RequestRevisionCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.Message).NotEmpty().MaximumLength(4000);
    }
}