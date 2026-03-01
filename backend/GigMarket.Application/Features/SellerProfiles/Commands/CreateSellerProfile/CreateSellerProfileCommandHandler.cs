using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.SellerProfiles.Models;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.SellerProfiles.Commands.CreateSellerProfile
{
    public sealed class CreateSellerProfileCommandHandler(ICurrentUserService currentUser, IApplicationDbContext db) : IRequestHandler<CreateSellerProfileCommand, SellerProfileDto>
    {
        public async Task<SellerProfileDto> Handle(CreateSellerProfileCommand request, CancellationToken ct)
        {
            if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.UserId.ToString()))
                throw new UnauthorizedException("Not authenticated.");

            var userId = currentUser.UserId!;

            var exists = await db.SellerProfiles.AnyAsync(x => x.UserId == userId, ct);
            if (exists) throw new BadRequestException("Seller profile already exists.");

            var entity = new SellerProfile
            {
                Id = Guid.NewGuid(),
                UserId = (Guid)userId,
                CreatedAtUtc = DateTime.UtcNow
            };

            db.SellerProfiles.Add(entity);
            await db.SaveChangesAsync(ct);

            return new SellerProfileDto(entity.Id, entity.UserId.ToString(), entity.CreatedAtUtc);
        }
    }
}
