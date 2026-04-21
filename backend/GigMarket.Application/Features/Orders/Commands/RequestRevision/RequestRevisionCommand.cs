using MediatR;
 
namespace GigMarket.Application.Features.Orders.Commands.RequestRevision;
 
public sealed record RequestRevisionCommand(
    Guid OrderId,
    string Message) : IRequest;