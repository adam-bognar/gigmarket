namespace GigMarket.Domain.Entities;

public class OrderDeliveryAttachment
{
    public Guid Id { get; set; }

    public Guid OrderDeliveryId { get; set; }
    public virtual OrderDelivery OrderDelivery { get; set; } = null!;

    public string FileUrl { get; set; } = string.Empty;

    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long? SizeInBytes { get; set; }

    public int SortOrder { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}