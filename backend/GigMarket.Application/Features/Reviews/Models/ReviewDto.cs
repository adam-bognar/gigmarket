namespace GigMarket.Application.Features.Reviews.Models;

public sealed record ReviewDto(
    Guid Id,
    Guid GigId,
    Guid ReviewerUserId,
    string ReviewerUsername,
    int Rating,
    string Description,
    DateTime CreatedAtUtc
);

