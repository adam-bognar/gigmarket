using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.Auth.Commands.Logout
{
    public sealed record LogoutCommand : IRequest;
}
