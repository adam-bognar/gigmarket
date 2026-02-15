using GigMarket.Application.Common.Exceptions;
using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Auth.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.Auth.Queries.Me
{
    public sealed class MeQueryHandler(ICurrentUserService currentUser, IIdentityService identityService)
    : IRequestHandler<MeQuery, AuthUserDto>
    {
        public async Task<AuthUserDto> Handle(MeQuery request, CancellationToken ct)
        {
            if (!currentUser.IsAuthenticated || string.IsNullOrWhiteSpace(currentUser.UserId.ToString()))
                throw new UnauthorizedException("Not authenticated.");

            return await identityService.GetByIdAsync((Guid)currentUser.UserId!, ct);
        }
    }
}
