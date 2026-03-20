namespace GigMarket.Domain.Entities;

public class GigPhoto
{
    public Guid Id { get; set; }
    public Guid GigId { get; set; }
    public virtual Gig Gig { get; set; }
    public string Url { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}