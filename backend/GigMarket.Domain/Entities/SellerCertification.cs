namespace GigMarket.Domain.Entities;

public class SellerCertification
{
    public Guid Id { get; set; }
    
    public Guid SellerProfileId { get; set; }
    public virtual SellerProfile SellerProfile { get; set; }
    
    public string Name { get; set; }
    public string IssuingOrganization { get; set; }
    public DateTime IssueDate { get; set; }
}