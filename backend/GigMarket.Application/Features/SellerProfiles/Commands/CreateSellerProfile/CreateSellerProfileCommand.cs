using GigMarket.Application.Features.SellerProfiles.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.SellerProfiles.Commands.CreateSellerProfile
{
    public sealed record CreateSellerProfileCommand(string DisplayName, string? Bio) : IRequest<SellerProfileDto>;
}
