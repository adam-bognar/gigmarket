namespace GigMarket.Domain.Entities;

public class GigVideo
{
    public Guid Id { get; set; }
    public Guid GigId { get; set; }
    public virtual Gig Gig { get; set; }
    public string Url { get; set; }
}