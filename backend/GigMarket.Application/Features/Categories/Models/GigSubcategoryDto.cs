namespace GigMarket.Application.Features.Categories.Models;

public sealed record GigSubcategoryDto(
    Guid Id,
    Guid CategoryId,
    string Name
);

