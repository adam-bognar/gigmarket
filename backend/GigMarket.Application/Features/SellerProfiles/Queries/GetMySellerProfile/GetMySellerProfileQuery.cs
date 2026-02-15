using GigMarket.Application.Features.SellerProfiles.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.SellerProfiles.Queries.GetMySellerProfile
{
    public sealed record GetMySellerProfileQuery : IRequest<SellerProfileDto>;
}
