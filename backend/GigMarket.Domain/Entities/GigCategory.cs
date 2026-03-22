namespace GigMarket.Domain.Entities;

public class GigCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<GigSubcategory> Subcategories { get; set; } = new List<GigSubcategory>();
    public virtual ICollection<Gig> Gigs { get; set; } = new List<Gig>();
}

