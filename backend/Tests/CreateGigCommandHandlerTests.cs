using FluentAssertions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Gigs.Commands.CreateGig;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;
using NSubstitute;

namespace Tests;

public class CreateGigCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_GigService_CreateGigAsync()
    {
        var gigService = Substitute.For<IGigService>();
        var handler = new CreateGigCommandHandler(gigService);

        var request = CreateValidRequest();
        var command = new CreateGigCommand(request);

        var expectedGig = CreateGigDto();

        gigService
            .CreateGigAsync(request, Arg.Any<CancellationToken>())
            .Returns(expectedGig);

        await handler.Handle(command, CancellationToken.None);

        await gigService
            .Received(1)
            .CreateGigAsync(request, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task Handle_Should_Return_GigDto_From_GigService()
    {
        var gigService = Substitute.For<IGigService>();
        var handler = new CreateGigCommandHandler(gigService);

        var request = CreateValidRequest();
        var command = new CreateGigCommand(request);

        var expectedGig = CreateGigDto();

        gigService
            .CreateGigAsync(request, Arg.Any<CancellationToken>())
            .Returns(expectedGig);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().BeEquivalentTo(expectedGig);
    }
    
    private static CreateGigRequest CreateValidRequest()
    {
        return new CreateGigRequest
        {
            GigId = Guid.NewGuid(),
            Title = "I will build a modern landing page",
            CategoryId = Guid.NewGuid(),
            SubcategoryId = Guid.NewGuid(),
            Tags = ["angular", "frontend"],
            Description = new string('a', 120),
            Packages =
            [
                new GigPackageRequest
                {
                    Tier = PackageTier.Basic,
                    Name = "Basic package",
                    Description = "This package contains a complete and valid service description.",
                    DeliveryDays = 7,
                    Revisions = 1,
                    Price = 25
                }
            ],
            Requirements =
            [
                new GigRequirementRequest
                {
                    Type = RequirementType.FreeText,
                    Question = "Please describe your project.",
                    IsRequired = true,
                    SortOrder = 1,
                    Choices = []
                }
            ],
            PrimaryPhotoUrl = "primary-photo.jpg",
            AdditionalPhotoUrls = ["additional-photo-1.jpg"],
            VideoUrl = null
        };
    }

    private static GigDto CreateGigDto()
    {
        return new GigDto
        {
            Id = Guid.NewGuid(),
            SellerProfileId = Guid.NewGuid(),
            Title = "I will build a modern landing page",
            Category = "Programming",
            Subcategory = "Web Development",
            Status = "Active",
            CreatedAtUtc = DateTime.UtcNow,
            Tags = ["angular", "frontend"],
            Packages =
            [
                new GigPackageDto
                {
                    Id = Guid.NewGuid(),
                    Tier = "Basic",
                    Name = "Basic package",
                    Description = "This package contains a complete and valid service description.",
                    DeliveryDays = 7,
                    Revisions = 1,
                    Price = 25
                }
            ],
            Photos = new GigPhotosDto
            {
                PrimaryPhotoUrl = "primary-photo.jpg",
                AdditionalPhotoUrls = ["additional-photo-1.jpg"]
            }
        };
    }
}