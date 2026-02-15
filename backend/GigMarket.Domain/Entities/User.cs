using Microsoft.AspNetCore.Identity;

namespace GigMarket.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public SellerProfile? SellerProfile { get; set; }
}
