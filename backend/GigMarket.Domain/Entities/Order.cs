namespace GigMarket.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }

    public Guid GigId { get; set; }
    public virtual Gig Gig { get; set; } = null!;

    public Guid PackageId { get; set; }
    public virtual GigPackage Package { get; set; } = null!;

    public Guid BuyerUserId { get; set; }
    public virtual User Buyer { get; set; } = null!;

    public string StripeSessionId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.InProgress;
    public decimal TotalPrice { get; set; }
    public int RevisionsUsed { get; set; } = 0;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAtUtc { get; set; }
    public DateTime? DeadlineUtc { get; set; }

    public virtual ICollection<OrderDelivery> Deliveries { get; set; } = new List<OrderDelivery>();
    public virtual ICollection<OrderRevisionRequest> RevisionRequests { get; set; } = new List<OrderRevisionRequest>();
}