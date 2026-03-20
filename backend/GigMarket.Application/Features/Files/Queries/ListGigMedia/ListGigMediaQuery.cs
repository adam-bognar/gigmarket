using MediatR;

namespace GigMarket.Application.Features.Files.Queries.ListGigMedia;

public sealed record ListGigMediaQuery(Guid GigId) : IRequest<List<string>>;

