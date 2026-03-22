namespace GigMarket.Application.Features.Reviews.Models;

public sealed record ReviewDto(
    Guid Id,
    Guid GigId,
    Guid ReviewerUserId,
    string ReviewerFirstName,
    string ReviewerLastName,
    int Rating,
    string Description,
    DateTime CreatedAtUtc
);

