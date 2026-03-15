namespace GigMarket.Domain.Entities;

public class GigPackage
{
    public Guid Id { get; set; }
    public Guid GigId { get; set; }
    public virtual Gig Gig { get; set; }

    public PackageTier Tier { get; set; }
    public string Name { get; set; }
    public  string Description { get; set; }
    public int DeliveryDays { get; set; }
    public int Revisions { get; set; }
    public decimal Price { get; set; }
}