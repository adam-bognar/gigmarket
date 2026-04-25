using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Auth.Models;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GigMarket.Application.Features.Auth.Commands.UpdateAccount;

public sealed class UpdateAccountCommandHandler(
    ICurrentUserService currentUser,
    IIdentityService identityService,
    UserManager<User> userManager)
    : IRequestHandler<UpdateAccountCommand, AuthUserDto>
{
    public async Task<AuthUserDto> Handle(UpdateAccountCommand request, CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
            throw new UnauthorizedException("Not authenticated.");

        var user = await userManager.FindByIdAsync(currentUser.UserId.Value.ToString());
        if (user is null) throw new NotFoundException("User not found.");

        if (!string.Equals(user.CustomUsername, request.CustomUsername, StringComparison.OrdinalIgnoreCase))
        {
            var existingByUsername = await userManager.FindByNameAsync(request.CustomUsername);
            if (existingByUsername is not null && existingByUsername.Id != user.Id)
                throw new BadRequestException("Username is already taken.");
        }

        if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingByEmail = await userManager.FindByEmailAsync(request.Email);
            if (existingByEmail is not null && existingByEmail.Id != user.Id)
                throw new BadRequestException("Email is already taken.");

            var emailResult = await userManager.SetEmailAsync(user, request.Email);
            if (!emailResult.Succeeded)
                throw new BadRequestException("Failed to update email.", emailResult.Errors.Select(e => e.Description).ToArray());
        }

        if (!string.Equals(user.CustomUsername, request.CustomUsername, StringComparison.OrdinalIgnoreCase))
        {
            user.CustomUsername = request.CustomUsername;
            var usernameResult = await userManager.SetUserNameAsync(user, request.CustomUsername);
            if (!usernameResult.Succeeded)
                throw new BadRequestException("Failed to update username.", usernameResult.Errors.Select(e => e.Description).ToArray());

            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new BadRequestException("Failed to update account.", updateResult.Errors.Select(e => e.Description).ToArray());
        }

        return await identityService.GetByIdAsync(user.Id, ct);
    }
}