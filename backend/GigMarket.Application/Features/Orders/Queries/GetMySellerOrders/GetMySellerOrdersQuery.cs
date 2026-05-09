using GigMarket.Application.Features.Orders.Models;
using MediatR;

namespace GigMarket.Application.Features.Orders.Queries.GetMySellerOrders;

public sealed record GetMySellerOrdersQuery : IRequest<List<OrderDto>>;