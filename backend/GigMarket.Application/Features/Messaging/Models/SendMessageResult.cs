using GigMarket.Application.Features.Messaging.Models;

namespace GigMarket.Application.Features.Messaging.Models;

public sealed record SendMessageResult(
    MessageDto Message,
    Guid RecipientUserId
);