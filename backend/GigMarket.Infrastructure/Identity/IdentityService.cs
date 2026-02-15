using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Auth.Models;
using GigMarket.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Infrastructure.Identity
{
    public sealed class IdentityService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IApplicationDbContext db) : IIdentityService
    {
        public async Task<AuthUserDto> RegisterAsync(string email, string password, CancellationToken ct)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null) throw new BadRequestException("Email is already taken.");

            var user = new User { UserName = email, Email = email };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new BadRequestException("Registration failed.", result.Errors.Select(e => e.Description).ToArray());

            await signInManager.SignInAsync(user, isPersistent: true);

            return new AuthUserDto(user.Id, user.Email ?? email, IsSeller: false);
        }

        public async Task<AuthUserDto> LoginAsync(string email, string password, bool isPersistent, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) throw new UnauthorizedException("Invalid email or password.");

            var signInResult = await signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure: false);
            if (!signInResult.Succeeded) throw new UnauthorizedException("Invalid email or password.");

            var isSeller = await db.SellerProfiles.AnyAsync(x => x.UserId == user.Id, ct);
            return new AuthUserDto(user.Id, user.Email ?? email, isSeller);
        }

        public async Task LogoutAsync(CancellationToken ct)
            => await signInManager.SignOutAsync();

        public async Task<AuthUserDto> GetByIdAsync(Guid userId, CancellationToken ct)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) throw new NotFoundException("User not found.");

            var isSeller = await db.SellerProfiles.AnyAsync(x => x.UserId == user.Id, ct);
            return new AuthUserDto(user.Id, user.Email ?? user.UserName ?? "", isSeller);
        }
    }

}
