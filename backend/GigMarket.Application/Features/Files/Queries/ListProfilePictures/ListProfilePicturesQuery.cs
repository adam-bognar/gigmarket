using MediatR;

namespace GigMarket.Application.Features.Files.Queries.ListProfilePictures;

public sealed record ListProfilePicturesQuery(Guid UserId) : IRequest<List<string>>;

