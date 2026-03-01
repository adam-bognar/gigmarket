using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.SellerProfiles.Models
{
    public sealed record SellerProfileDto(
    Guid Id,
    string UserId,
    DateTime CreatedAtUtc);
}
