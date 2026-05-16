using FluentAssertions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Infrastructure.Service;
using NSubstitute;
using Stripe.Checkout;

namespace Tests;

public class StripeCheckoutServiceTests
{
    private const string TestSessionUrl = "https://checkout.stripe.com/test-session";
    private const string DefaultGigTitle = "I will build a website";
    private const string DefaultPackageName = "Basic package";
    private const string DefaultPackageDescription = "Basic package description";
    private const string DefaultClientBaseUrl = "http://localhost:4200";
    private const decimal DefaultPrice = 25;

    private static IStripeSessionService CreateStripeSessionService(
        Action<SessionCreateOptions>? captureOptions = null)
    {
        var stripeSessionService = Substitute.For<IStripeSessionService>();

        var setup = stripeSessionService.CreateAsync(
            captureOptions is null
                ? Arg.Any<SessionCreateOptions>()
                : Arg.Do(captureOptions),
            Arg.Any<CancellationToken>());

        setup.Returns(new Session { Url = TestSessionUrl });

        return stripeSessionService;
    }

    private static Task<string> InvokeAsync(
        StripeCheckoutService service,
        Guid? gigId = null,
        Guid? packageId = null,
        Guid? buyerUserId = null,
        string? primaryImageUrl = null,
        CancellationToken cancellationToken = default)
        => service.CreateCheckoutSessionAsync(
            gigTitle: DefaultGigTitle,
            packageName: DefaultPackageName,
            packageDescription: DefaultPackageDescription,
            primaryImageUrl: primaryImageUrl,
            price: DefaultPrice,
            gigId: gigId ?? Guid.NewGuid(),
            packageId: packageId ?? Guid.NewGuid(),
            buyerUserId: buyerUserId ?? Guid.NewGuid(),
            clientBaseUrl: DefaultClientBaseUrl,
            cancellationToken: cancellationToken);

    [Fact]
    public async Task CreateCheckoutSessionAsync_Should_Return_Stripe_Session_Url()
    {
        var stripeSessionService = CreateStripeSessionService();
        var service = new StripeCheckoutService(stripeSessionService);

        var result = await InvokeAsync(service, primaryImageUrl: "https://example.com/photo.jpg");

        result.Should().Be(TestSessionUrl);
    }

    [Fact]
    public async Task CreateCheckoutSessionAsync_Should_Set_Correct_Price_In_Cents()
    {
        SessionCreateOptions? capturedOptions = null;
        var service = new StripeCheckoutService(CreateStripeSessionService(o => capturedOptions = o));

        await InvokeAsync(service);

        capturedOptions.Should().NotBeNull();

        var lineItem = capturedOptions!.LineItems.Single();

        lineItem.PriceData!.UnitAmount.Should().Be(2500);
        lineItem.PriceData.Currency.Should().Be("usd");
        lineItem.Quantity.Should().Be(1);
    }

    [Fact]
    public async Task CreateCheckoutSessionAsync_Should_Set_Payment_Mode_And_Card_Payment_Method()
    {
        SessionCreateOptions? capturedOptions = null;
        var service = new StripeCheckoutService(CreateStripeSessionService(o => capturedOptions = o));

        await InvokeAsync(service);

        capturedOptions.Should().NotBeNull();

        capturedOptions!.Mode.Should().Be("payment");
        capturedOptions.PaymentMethodTypes.Should().ContainSingle();
        capturedOptions.PaymentMethodTypes.Single().Should().Be("card");
    }

    [Fact]
    public async Task CreateCheckoutSessionAsync_Should_Set_Success_And_Cancel_Urls()
    {
        SessionCreateOptions? capturedOptions = null;
        var service = new StripeCheckoutService(CreateStripeSessionService(o => capturedOptions = o));

        var gigId = Guid.NewGuid();

        await InvokeAsync(service, gigId: gigId);

        capturedOptions.Should().NotBeNull();

        capturedOptions!.SuccessUrl
            .Should()
            .Be("http://localhost:4200/orders/success?session_id={CHECKOUT_SESSION_ID}");

        capturedOptions.CancelUrl
            .Should()
            .Be($"http://localhost:4200/gigs/{gigId}");
    }

    [Fact]
    public async Task CreateCheckoutSessionAsync_Should_Set_Metadata()
    {
        SessionCreateOptions? capturedOptions = null;
        var service = new StripeCheckoutService(CreateStripeSessionService(o => capturedOptions = o));

        var gigId = Guid.NewGuid();
        var packageId = Guid.NewGuid();
        var buyerUserId = Guid.NewGuid();

        await InvokeAsync(service, gigId: gigId, packageId: packageId, buyerUserId: buyerUserId);

        capturedOptions.Should().NotBeNull();

        capturedOptions!.Metadata["gigId"].Should().Be(gigId.ToString());
        capturedOptions.Metadata["packageId"].Should().Be(packageId.ToString());
        capturedOptions.Metadata["buyerUserId"].Should().Be(buyerUserId.ToString());
    }

    [Fact]
    public async Task CreateCheckoutSessionAsync_Should_Include_Image_When_PrimaryImageUrl_Is_Provided()
    {
        SessionCreateOptions? capturedOptions = null;
        var service = new StripeCheckoutService(CreateStripeSessionService(o => capturedOptions = o));

        await InvokeAsync(service, primaryImageUrl: "https://example.com/photo.jpg");

        capturedOptions.Should().NotBeNull();

        var productData = capturedOptions!
            .LineItems
            .Single()
            .PriceData!
            .ProductData!;

        productData.Images.Should().ContainSingle();
        productData.Images.Single().Should().Be("https://example.com/photo.jpg");
    }

    [Fact]
    public async Task CreateCheckoutSessionAsync_Should_Not_Include_Image_When_PrimaryImageUrl_Is_Null()
    {
        SessionCreateOptions? capturedOptions = null;
        var service = new StripeCheckoutService(CreateStripeSessionService(o => capturedOptions = o));

        await InvokeAsync(service);

        capturedOptions.Should().NotBeNull();

        var productData = capturedOptions!
            .LineItems
            .Single()
            .PriceData!
            .ProductData!;

        productData.Images.Should().BeNull();
    }
}