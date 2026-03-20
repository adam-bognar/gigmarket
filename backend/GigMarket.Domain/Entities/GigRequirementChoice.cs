namespace GigMarket.Domain.Entities;

public class GigRequirementChoice
{
    public Guid Id { get; set; }
    public Guid GigRequirementId { get; set; }
    public virtual GigRequirement GigRequirement { get; set; }
    public string Value { get; set; }
}