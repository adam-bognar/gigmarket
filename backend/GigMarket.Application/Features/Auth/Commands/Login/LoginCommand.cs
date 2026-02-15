using GigMarket.Application.Features.Auth.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.Auth.Commands.Login
{
    public sealed record LoginCommand(string Email, string Password, bool RememberMe) : IRequest<AuthUserDto>;

}
