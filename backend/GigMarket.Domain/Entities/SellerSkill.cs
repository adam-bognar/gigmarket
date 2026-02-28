namespace GigMarket.Domain.Entities;

public class SellerSkill
{
    public Guid SellerProfileId { get; set; }
    public virtual SellerProfile SellerProfile { get; set; }
    
    public Guid SkillId { get; set; }
    public virtual Skill Skill { get; set; }
}