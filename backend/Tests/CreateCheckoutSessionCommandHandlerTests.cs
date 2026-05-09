using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Orders.Commands.CreateCheckoutSession;
using GigMarket.Domain.Entities;
using GigMarket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Tests;

public class CreateCheckoutSessionCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Throw_UnauthorizedException_When_User_Is_Not_Authenticated()
    {
        await using var db = CreateDbContext();

        var currentUser = Substitute.For<ICurrentUserService>();
        var configuration = Substitute.For<IConfiguration>();
        var stripeCheckoutService = Substitute.For<IStripeCheckoutService>();

        currentUser.IsAuthenticated.Returns(false);

        var handler = new CreateCheckoutSessionCommandHandler(
            db,
            currentUser,
            configuration,
            stripeCheckoutService);

        var command = new CreateCheckoutSessionCommand(Guid.NewGuid(), Guid.NewGuid());

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            handler.Handle(command, CancellationToken.None));

        await stripeCheckoutService
            .Received(0)
            .CreateCheckoutSessionAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<decimal>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFoundException_When_Package_Does_Not_Exist_On_Gig()
    {
        await using var db = CreateDbContext();

        var currentUser = Substitute.For<ICurrentUserService>();
        var configuration = Substitute.For<IConfiguration>();
        var stripeCheckoutService = Substitute.For<IStripeCheckoutService>();

        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns(Guid.NewGuid());

        var handler = new CreateCheckoutSessionCommandHandler(
            db,
            currentUser,
            configuration,
            stripeCheckoutService);

        var command = new CreateCheckoutSessionCommand(Guid.NewGuid(),Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));

        await stripeCheckoutService
            .Received(0)
            .CreateCheckoutSessionAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<decimal>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Buyer_Tries_To_Purchase_Own_Gig()
    {
        await using var db = CreateDbContext();

        var sellerUserId = Guid.NewGuid();
        var gigId = Guid.NewGuid();
        var packageId = Guid.NewGuid();

        SeedGigPackage(db, gigId, packageId, sellerUserId);

        var currentUser = Substitute.For<ICurrentUserService>();
        var configuration = Substitute.For<IConfiguration>();
        var stripeCheckoutService = Substitute.For<IStripeCheckoutService>();

        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns(sellerUserId);

        configuration["ClientBaseUrl"].Returns("http://localhost:4200");
        
        var handler = new CreateCheckoutSessionCommandHandler(
            db,
            currentUser,
            configuration,
            stripeCheckoutService);

        var command = new CreateCheckoutSessionCommand(
            gigId,
            packageId);

        await Assert.ThrowsAsync<BadRequestException>(() =>
            handler.Handle(command, CancellationToken.None));

        await stripeCheckoutService
            .Received(0)
            .CreateCheckoutSessionAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<decimal>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Throw_InvalidOperationException_When_ClientBaseUrl_Is_Not_Configured()
    {
        await using var db = CreateDbContext();

        var sellerUserId = Guid.NewGuid();
        var buyerUserId = Guid.NewGuid();
        var gigId = Guid.NewGuid();
        var packageId = Guid.NewGuid();

        SeedGigPackage(db, gigId, packageId, sellerUserId);

        var currentUser = Substitute.For<ICurrentUserService>();
        var configuration = Substitute.For<IConfiguration>();
        var stripeCheckoutService = Substitute.For<IStripeCheckoutService>();

        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns(buyerUserId);

        configuration["ClientBaseUrl"].Returns((string?)null);

        var handler = new CreateCheckoutSessionCommandHandler(
            db,
            currentUser,
            configuration,
            stripeCheckoutService);

        var command = new CreateCheckoutSessionCommand(
            gigId,
            packageId);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(command, CancellationToken.None));

        await stripeCheckoutService
            .Received(0)
            .CreateCheckoutSessionAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<decimal>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Call_StripeCheckoutService_And_Return_CheckoutUrl_When_Request_Is_Valid()
    {
        await using var db = CreateDbContext();

        var sellerUserId = Guid.NewGuid();
        var buyerUserId = Guid.NewGuid();
        var gigId = Guid.NewGuid();
        var packageId = Guid.NewGuid();

        SeedGigPackage(db, gigId, packageId, sellerUserId);

        var currentUser = Substitute.For<ICurrentUserService>();
        var configuration = Substitute.For<IConfiguration>();
        var stripeCheckoutService = Substitute.For<IStripeCheckoutService>();

        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns(buyerUserId);

        configuration["ClientBaseUrl"].Returns("http://localhost:4200");

        stripeCheckoutService
            .CreateCheckoutSessionAsync(
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string?>(),
                Arg.Any<decimal>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<Guid>(),
                Arg.Any<string>(),
                Arg.Any<CancellationToken>())
            .Returns("https://checkout.stripe.com/test-session");

        var handler = new CreateCheckoutSessionCommandHandler(
            db,
            currentUser,
            configuration,
            stripeCheckoutService);

        var command = new CreateCheckoutSessionCommand(
            GigId: gigId,
            PackageId: packageId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal("https://checkout.stripe.com/test-session", result);

        await stripeCheckoutService
            .Received(1)
            .CreateCheckoutSessionAsync(
                "I will build a website",
                "Basic package",
                "Basic package description",
                "https://example.com/photo.jpg",
                25,
                gigId,
                packageId,
                buyerUserId,
                "http://localhost:4200",
                Arg.Any<CancellationToken>());
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static void SeedGigPackage(
        ApplicationDbContext db,
        Guid gigId,
        Guid packageId,
        Guid sellerUserId)
    {
        var sellerProfileId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var subcategoryId = Guid.NewGuid();

        var sellerProfile = new SellerProfile
        {
            Id = sellerProfileId,
            UserId = sellerUserId,
            FirstName = "Adam",
            LastName = "Seller",
            Description = "Seller description",
            ProfileImageUrl = "profile.jpg"
        };

        var category = new GigCategory
        {
            Id = categoryId,
            Name = $"Category-{Guid.NewGuid()}"
        };

        var subcategory = new GigSubcategory
        {
            Id = subcategoryId,
            CategoryId = categoryId,
            Name = $"Subcategory-{Guid.NewGuid()}"
        };

        var gig = new Gig
        {
            Id = gigId,
            SellerProfileId = sellerProfileId,
            CategoryId = categoryId,
            SubcategoryId = subcategoryId,
            Title = "I will build a website",
            Description = "This is a valid gig description.",
            Status = GigStatus.Active
        };

        var package = new GigPackage
        {
            Id = packageId,
            GigId = gigId,
            Tier = PackageTier.Basic,
            Name = "Basic package",
            Description = "Basic package description",
            DeliveryDays = 7,
            Revisions = 1,
            Price = 25
        };

        var photo = new GigPhoto
        {
            Id = Guid.NewGuid(),
            GigId = gigId,
            Url = "https://example.com/photo.jpg",
            IsPrimary = true,
            SortOrder = 1
        };

        db.SellerProfiles.Add(sellerProfile);
        db.GigCategories.Add(category);
        db.GigSubcategories.Add(subcategory);
        db.Gigs.Add(gig);
        db.GigPackages.Add(package);
        db.GigPhotos.Add(photo);

        db.SaveChanges();
    }
}