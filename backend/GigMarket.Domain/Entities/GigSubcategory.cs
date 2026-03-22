namespace GigMarket.Domain.Entities;

public class GigSubcategory
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public virtual GigCategory Category { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Gig> Gigs { get; set; } = new List<Gig>();
}

