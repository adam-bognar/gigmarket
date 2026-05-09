using FluentValidation.TestHelper;
using GigMarket.Application.Features.Gigs.Commands.CreateGig;
using GigMarket.Domain.Entities;

namespace Tests;

public class CreateGigCommandValidatorTests
{
    private readonly CreateGigCommandValidator _validator = new();
    
    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        var request = CreateValidRequest() with
        {
            Title = ""
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.Title);
    }
    
    [Fact]
    public void Should_Have_Error_When_Title_Is_Too_Long()
    {
        var request = CreateValidRequest() with
        {
            Title = new string('a', 101)
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.Title);
    }
    
    [Fact]
    public void Should_Have_Error_When_CategoryId_Is_Empty()
    {
        var request = CreateValidRequest() with
        {
            CategoryId = Guid.Empty
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.CategoryId);
    }
    
    [Fact]
    public void Should_Have_Error_When_SubcategoryId_Is_Empty()
    {
        var request = CreateValidRequest() with
        {
            SubcategoryId = Guid.Empty
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.SubcategoryId);
    }
    
    [Fact]
    public void Should_Have_Error_When_Description_Is_Empty()
    {
        var request = CreateValidRequest() with
        {
            Description = ""
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.Description);
    }
    
    [Fact]
    public void Should_Have_Error_When_Description_Is_Too_Short()
    {
        var request = CreateValidRequest() with
        {
            Description = new string('a', 119)
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.Description);
    }
    
    [Fact]
    public void Should_Have_Error_When_Tags_Are_Empty()
    {
        var request = CreateValidRequest() with
        {
            Tags = []
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.Tags);
    }
    
    [Fact]
    public void Should_Have_Error_When_There_Are_More_Than_Five_Tags()
    {
        var request = CreateValidRequest() with
        {
            Tags =
            [
                "angular",
                "aspnet",
                "web",
                "design",
                "backend",
                "frontend"
            ]
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.Tags);
    }
    
    [Fact]
    public void Should_Have_Error_When_Tag_Is_Empty()
    {
        var request = CreateValidRequest() with
        {
            Tags = ["angular", ""]
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor("GigRequest.Tags[1]");
    }

    [Fact]
    public void Should_Have_Error_When_Tag_Is_Too_Long()
    {
        var request = CreateValidRequest() with
        {
            Tags = ["angular", new string('a', 31)]
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor("GigRequest.Tags[1]");
    }
    
    [Fact]
    public void Should_Have_Error_When_Packages_Are_Empty()
    {
        var request = CreateValidRequest() with
        {
            Packages = []
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.Packages);
    }
    
    [Fact]
    public void Should_Have_Error_When_There_Are_More_Than_Three_Packages()
    {
        var request = CreateValidRequest() with
        {
            Packages =
            [
                CreateValidPackage(PackageTier.Basic),
                CreateValidPackage(PackageTier.Standard),
                CreateValidPackage(PackageTier.Premium),
                CreateValidPackage(PackageTier.Basic)
            ]
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.Packages);
    }
    
    [Fact]
    public void Should_Have_Error_When_Package_Price_Is_Less_Than_Five()
    {
        var request = CreateValidRequest() with
        {
            Packages =
            [
                CreateValidPackage(PackageTier.Basic) with
                {
                    Price = 4
                }
            ]
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor("GigRequest.Packages[0].Price");
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(91)]
    public void Should_Have_Error_When_DeliveryDays_Is_Outside_Allowed_Range(int deliveryDays)
    {
        var request = CreateValidRequest() with
        {
            Packages =
            [
                CreateValidPackage(PackageTier.Basic) with
                {
                    DeliveryDays = deliveryDays
                }
            ]
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor("GigRequest.Packages[0].DeliveryDays");
    }
    
    [Fact]
    public void Should_Have_Error_When_PrimaryPhotoUrl_Is_Empty()
    {
        var request = CreateValidRequest() with
        {
            PrimaryPhotoUrl = ""
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.PrimaryPhotoUrl);
    }
    
    [Fact]
    public void Should_Have_Error_When_There_Are_More_Than_Two_Additional_Photos()
    {
        var request = CreateValidRequest() with
        {
            AdditionalPhotoUrls =
            [
                "photo-1.jpg",
                "photo-2.jpg",
                "photo-3.jpg"
            ]
        };

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldHaveValidationErrorFor(x => x.GigRequest.AdditionalPhotoUrls);
    }
    
    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        var request = CreateValidRequest();

        var result = _validator.TestValidate(new CreateGigCommand(request));

        result.ShouldNotHaveAnyValidationErrors();
    }
    
    private static CreateGigRequest CreateValidRequest()
    {
        return new CreateGigRequest
        {
            Title = "I will build a modern landing page",
            CategoryId = Guid.NewGuid(),
            SubcategoryId = Guid.NewGuid(),
            Description = new string('a', 120),
            Tags = ["angular", "frontend"],
            PrimaryPhotoUrl = "primary-photo.jpg",
            AdditionalPhotoUrls = ["additional-photo-1.jpg"],
            Packages =
            [
                CreateValidPackage(PackageTier.Basic)
            ],
            Requirements =
            [
                new GigRequirementRequest
                {
                    Question = "Please describe your project.",
                    Type = RequirementType.FreeText,
                    IsRequired = true,
                    Choices = []
                }
            ]
        };
    }
    
    private static GigPackageRequest CreateValidPackage(PackageTier tier)
    {
        return new GigPackageRequest
        {
            Name = $"{tier} package",
            Description = "This package contains a complete and valid service description.",
            DeliveryDays = 7,
            Revisions = 1,
            Price = 25,
            Tier = tier
        };
    }
}