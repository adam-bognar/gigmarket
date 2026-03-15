namespace GigMarket.Domain.Entities;

public class GigTag
{
    public Guid Id { get; set; }
    public Guid GigId { get; set; }
    public virtual Gig Gig { get; set; }
    public string Name { get; set; }
}