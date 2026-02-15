using GigMarket.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.Auth.Commands.Logout
{
    public sealed class LogoutCommandHandler(IIdentityService identityService) : IRequestHandler<LogoutCommand>
    {
        public async Task Handle(LogoutCommand request, CancellationToken ct)
            => await identityService.LogoutAsync(ct);
    }
}
