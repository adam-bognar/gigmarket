namespace GigMarket.Domain.Entities;

public class SellerLanguage
{
    public SellerProfile SellerProfile { get; set; }
    public Guid SellerProfileId { get; set; }
    
    public Guid LanguageId { get; set; }
    public Language Language { get; set; }
}