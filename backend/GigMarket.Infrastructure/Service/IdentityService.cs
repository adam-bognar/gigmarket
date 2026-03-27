using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Auth.Models;
using GigMarket.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Infrastructure.Service
{
    public sealed class IdentityService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IApplicationDbContext db,
    IBlobStorageService blobStorageService) : IIdentityService
    {
        public async Task<AuthUserDto> RegisterAsync(string customUsername, string email, string password, CancellationToken ct)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null) throw new BadRequestException("Email is already taken.");

            var usernameTaken = await userManager.FindByNameAsync(customUsername);
            if (usernameTaken is not null) throw new BadRequestException("Username is already taken.");

            var user = new User { UserName = customUsername, Email = email, CustomUsername = customUsername };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new BadRequestException("Registration failed.", result.Errors.Select(e => e.Description).ToArray());

            await signInManager.SignInAsync(user, isPersistent: true);
            var profileUrl =  await GetProfileUrlAsync(user.Id, ct);

            return new AuthUserDto(user.Id, user.CustomUsername, user.Email ?? email, IsSeller: false, ProfileUrl: profileUrl);
        }

        public async Task<AuthUserDto> LoginAsync(string email, string password, bool isPersistent, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user is null) throw new UnauthorizedException("Invalid email or password.");

            var signInResult = await signInManager.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure: false);
            if (!signInResult.Succeeded) throw new UnauthorizedException("Invalid email or password.");

            var isSeller = await db.SellerProfiles.AnyAsync(x => x.UserId == user.Id, ct);
            var profileUrl = await GetProfileUrlAsync(user.Id, ct);
            return new AuthUserDto(
                user.Id,
                user.CustomUsername,
                user.Email ?? email,
                isSeller,
                profileUrl);
        }

        public async Task LogoutAsync(CancellationToken ct)
            => await signInManager.SignOutAsync();

        public async Task<AuthUserDto> GetByIdAsync(Guid userId, CancellationToken ct)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) throw new NotFoundException("User not found.");

            var isSeller = await db.SellerProfiles.AnyAsync(x => x.UserId == user.Id, ct);
            var profileUrl = await GetProfileUrlAsync(user.Id, ct);
            return new AuthUserDto(
                user.Id,
                user.CustomUsername,
                user.Email ?? user.UserName ?? "",
                isSeller,
                profileUrl);
        }

        private async Task<string?> GetProfileUrlAsync(Guid userId, CancellationToken ct)
        {
            var profileFolder = $"profiles/{userId}";
            var blobs = await blobStorageService.ListBlobsAsync(profileFolder, ct);
            var blobPath = blobs.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(blobPath)) return null;

            return await blobStorageService.GetDownloadUrlAsync(blobPath, ct);
        }
    }

}
