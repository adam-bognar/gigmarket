using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Auth.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.Auth.Commands.Login
{
    public sealed class LoginCommandHandler(IIdentityService identityService) : IRequestHandler<LoginCommand, AuthUserDto>
    {
        public Task<AuthUserDto> Handle(LoginCommand request, CancellationToken ct)
            => identityService.LoginAsync(request.Email, request.Password, request.RememberMe, ct);
    }
}
