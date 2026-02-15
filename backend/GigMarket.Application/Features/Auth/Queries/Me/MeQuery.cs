using GigMarket.Application.Features.Auth.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.Auth.Queries.Me
{
    public sealed record MeQuery : IRequest<AuthUserDto>;
}
