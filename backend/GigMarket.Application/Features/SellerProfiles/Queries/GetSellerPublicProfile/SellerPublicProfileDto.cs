using GigMarket.Application.Features.Gigs.Models;
using GigMarket.Application.Features.SellerProfiles.Models;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetSellerPublicProfile;

public sealed record SellerPublicProfileDto(
    Guid Id,
    Guid UserId,
    string FirstName,
    string LastName,
    string Description,
    string ProfileImageUrl,
    string? PersonalWebsite,
    SellerOccupationDto Occupation,
    List<SellerLanguageDto> Languages,
    List<string> Skills,
    List<SellerEducationDto> Educations,
    List<SellerCertificationDto> Certifications,
    DateTime MemberSinceUtc,
    double AverageRating,
    int TotalReviews,
    List<GigSummaryDto> Gigs,
    List<SellerReviewDto> Reviews
);

public sealed record SellerReviewDto(
    Guid Id,
    Guid GigId,
    string GigTitle,
    Guid ReviewerUserId,
    string ReviewerUsername,
    int Rating,
    string Description,
    DateTime CreatedAtUtc
);