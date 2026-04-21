namespace GigMarket.Domain.Entities;

public class OrderRevisionRequest
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; } = null!;

    public string Message { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}