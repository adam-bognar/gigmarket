using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Auth.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.Auth.Commands.Register
{
    public sealed class RegisterCommandHandler(IIdentityService identityService) : IRequestHandler<RegisterCommand, AuthUserDto>
    {
        public Task<AuthUserDto> Handle(RegisterCommand request, CancellationToken ct)
            => identityService.RegisterAsync(request.Firstname, request.Lastname,request.Email, request.Password, ct);
    }
}
