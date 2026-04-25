namespace GigMarket.Domain.Entities
{
    public class SellerProfile
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string ProfileImageUrl { get; set; } 
        public string? PersonalWebsite { get; set; }
        public SellerOccupation Occupation { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string? StripeAccountId { get; set; }
        public SellerStripeAccountStatus StripeAccountStatus { get; set; } = SellerStripeAccountStatus.NotConnected;

        public virtual ICollection<SellerLanguage> Languages { get; set; } = new List<SellerLanguage>();
        public virtual ICollection<SellerSkill> Skills { get; set; } = new List<SellerSkill>();
        public virtual ICollection<SellerEducation> Educations { get; set; } = new List<SellerEducation>();
        public virtual ICollection<SellerCertification> Certifications { get; set; } = new List<SellerCertification>();
        public virtual ICollection<Gig> Gigs { get; set; } = new List<Gig>();
    }
}