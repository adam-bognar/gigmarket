namespace GigMarket.Domain.Entities;

public class SellerEducation
{
    public Guid Id { get; set; }
    public Guid SellerProfileId { get; set; }
    public virtual SellerProfile SellerProfile { get; set; }
    
    public string Country { get; set; }
    public string InstitutionName { get; set; }
    public string Degree { get; set; }
    public string Major { get; set; }
    public int GraduationYear { get; set; } 
}