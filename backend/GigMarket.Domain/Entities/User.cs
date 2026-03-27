using Microsoft.AspNetCore.Identity;

namespace GigMarket.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string CustomUsername { get; set; } = string.Empty;
    
    public SellerProfile? SellerProfile { get; set; }
}
