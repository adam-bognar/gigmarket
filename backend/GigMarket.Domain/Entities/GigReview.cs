namespace GigMarket.Domain.Entities;

public class GigReview
{
    public Guid Id { get; set; }

    public Guid GigId { get; set; }
    public virtual Gig Gig { get; set; } = null!;

    public Guid ReviewerUserId { get; set; }
    public virtual User Reviewer { get; set; } = null!;

    public int Rating { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

