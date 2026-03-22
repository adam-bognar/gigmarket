using GigMarket.Application.Features.Reviews.Models;
using MediatR;

namespace GigMarket.Application.Features.Reviews.Commands.AddGigReview;

public sealed record AddGigReviewCommand(AddGigReviewRequest Request) : IRequest<ReviewDto>;

public sealed record AddGigReviewRequest(
    Guid GigId,
    int Rating,
    string Description
);

