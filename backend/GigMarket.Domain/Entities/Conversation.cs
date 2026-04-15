namespace GigMarket.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }
    public Guid BuyerUserId { get; set; }
    public Guid SellerUserId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid GigId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime LastMessageAtUtc { get; set; }

    public User Buyer { get; set; } = null!;
    public User Seller { get; set; } = null!;
    public Gig Gig { get; set; } = null!;
    public ICollection<Message> Messages { get; set; } = [];
}