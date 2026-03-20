namespace GigMarket.Domain.Entities;

public class Gig
{
    public Guid Id { get; set; }
    public Guid SellerProfileId { get; set; }
    public virtual SellerProfile SellerProfile { get; set; }
    
    public string Title { get; set; }
    public string Category { get; set; }
    public string Subcategory { get; set; }
    public string Description { get; set; }
    public GigStatus Status { get; set; } = GigStatus.Draft;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<GigTag> Tags { get; set; } = new List<GigTag>();
    public virtual ICollection<GigPackage> Packages { get; set; } = new List<GigPackage>();
    public virtual ICollection<GigRequirement> Requirements { get; set; } = new List<GigRequirement>();
    public virtual ICollection<GigPhoto> Photos { get; set; } = new List<GigPhoto>();
    public GigVideo? Video { get; set; }
}