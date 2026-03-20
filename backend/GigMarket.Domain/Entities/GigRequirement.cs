namespace GigMarket.Domain.Entities;

public class GigRequirement
{
    public Guid Id { get; set; }
    public Guid GigId { get; set; }
    public virtual Gig Gig { get; set; }

    public RequirementType Type { get; set; }
    public string Question { get; set; }
    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }

    public virtual ICollection<GigRequirementChoice> Choices { get; set; } = new List<GigRequirementChoice>();
}