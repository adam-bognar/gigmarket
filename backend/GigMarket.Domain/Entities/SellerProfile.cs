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

        public string Description { get; set; }
        public string? ProfileImageUrl { get; set; } 
        public string? PersonalWebsite { get; set; }
        public string? PrimaryOccupation { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        
        public virtual ICollection<SellerLanguage> Languages { get; set; }
        public virtual ICollection<SellerSkill> Skills { get; set; }
        public virtual ICollection<SellerEducation> Educations { get; set; }
        public virtual ICollection<SellerCertification> Certifications { get; set; }
    }
}