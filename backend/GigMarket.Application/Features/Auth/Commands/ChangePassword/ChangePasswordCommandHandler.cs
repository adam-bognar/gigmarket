using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GigMarket.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    ICurrentUserService currentUser,
    UserManager<User> userManager)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
            throw new UnauthorizedException("Not authenticated.");

        var user = await userManager.FindByIdAsync(currentUser.UserId.Value.ToString());
        if (user is null) throw new NotFoundException("User not found.");

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
            throw new BadRequestException("Failed to change password.", result.Errors.Select(e => e.Description).ToArray());
    }
}