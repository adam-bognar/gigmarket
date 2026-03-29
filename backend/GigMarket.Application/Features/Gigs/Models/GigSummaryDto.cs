namespace GigMarket.Application.Features.Gigs.Models;
public sealed record GigSummaryDto(
    Guid Id,
    string Title,
    string PrimaryPhotoUrl,
    decimal StartingPrice,
    int MinDeliveryDays,
    Guid SellerProfileId,
    string SellerFirstName,
    string SellerLastName,
    string SellerAvatarUrl,
    Guid CategoryId,
    string CategoryName,
    string SubcategoryName,
    double AverageRating,
    int TotalReviews,
    List<string> Tags,
    string Status
);