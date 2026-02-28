using Microsoft.AspNetCore.Identity;

namespace GigMarket.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    
    public SellerProfile? SellerProfile { get; set; }
}
