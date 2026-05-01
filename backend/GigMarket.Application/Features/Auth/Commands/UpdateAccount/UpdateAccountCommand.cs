using GigMarket.Application.Features.Auth.Models;
using MediatR;

namespace GigMarket.Application.Features.Auth.Commands.UpdateAccount;

public sealed record UpdateAccountCommand(string CustomUsername) : IRequest<AuthUserDto>;