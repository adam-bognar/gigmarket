using GigMarket.Application.Features.Orders.Models;
using MediatR;

namespace GigMarket.Application.Features.Orders.Queries.GetMyOrders;

public sealed record GetMyOrdersQuery : IRequest<List<OrderDto>>;