using GigMarket.Application.Features.Orders.Models;
using MediatR;

namespace GigMarket.Application.Features.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid Id) : IRequest<OrderDetailDto>;