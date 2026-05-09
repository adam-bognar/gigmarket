using MediatR;

namespace GigMarket.Application.Features.SellerProfiles.Commands.ConnectStripeAccount;

public sealed record ConnectStripeAccountCommand : IRequest<ConnectStripeAccountResult>;

public sealed record ConnectStripeAccountResult(string? OnboardingUrl, string Status);