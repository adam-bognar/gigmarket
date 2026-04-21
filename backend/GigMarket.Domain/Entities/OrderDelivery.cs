namespace GigMarket.Domain.Entities;

public class OrderDelivery
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public string Message { get; set; } = string.Empty;

    public virtual ICollection<OrderDeliveryAttachment> Attachments { get; set; } = new List<OrderDeliveryAttachment>();

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}