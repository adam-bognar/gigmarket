using System;
using System.Collections.Generic;
using System.Text;

namespace GigMarket.Domain.Entities
{
    public class SellerProfile
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string DisplayName { get; set; } = null!;
        public string? Bio { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
