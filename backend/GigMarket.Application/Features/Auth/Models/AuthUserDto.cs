using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Application.Features.Auth.Models
{
    public sealed record AuthUserDto(Guid Id, string Email, bool IsSeller);
}
