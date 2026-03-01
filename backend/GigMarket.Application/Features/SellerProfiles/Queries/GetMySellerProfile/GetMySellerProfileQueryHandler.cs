using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.SellerProfiles.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetMySellerProfile
{
    public sealed class GetMySellerProfileQueryHandler(ICurrentUserService currentUser, IApplicationDbContext db)
    : IRequestHandler<GetMySellerProfileQuery, SellerProfileDto>
    {
        public async Task<SellerProfileDto> Handle(GetMySellerProfileQuery request, CancellationToken ct)
        {
            if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.UserId.ToString()))
                throw new UnauthorizedException("Not authenticated.");

            var userId = currentUser.UserId!;

            var entity = await db.SellerProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId, ct);

            if (entity is null) throw new NotFoundException("Seller profile not found.");

            return new SellerProfileDto(entity.Id, entity.UserId.ToString(), entity.CreatedAtUtc);
        }
    }
}
