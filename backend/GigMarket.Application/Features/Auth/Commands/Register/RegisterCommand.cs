using GigMarket.Application.Features.Auth.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.Auth.Commands.Register
{
    public sealed record RegisterCommand(string Firstname, string Lastname, string Email, string Password) : IRequest<AuthUserDto>;
}
