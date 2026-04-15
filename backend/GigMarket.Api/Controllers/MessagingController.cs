using GigMarket.Application.Features.Messaging.Commands.MarkConversationRead;
using GigMarket.Application.Features.Messaging.Commands.SendMessage;
using GigMarket.Application.Features.Messaging.Commands.StartConversation;
using GigMarket.Application.Features.Messaging.Queries.GetConversationMessages;
using GigMarket.Application.Features.Messaging.Queries.GetMyConversations;
using GigMarket.Application.Features.Messaging.Queries.GetUnreadCount;
using GigMarket.API.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GigMarket.API.Controllers;

[Authorize]
[ApiController]
[Route("api/conversations")]
public sealed class MessagingController(
    ISender mediator,
    IHubContext<ChatHub> hub) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new StartConversationCommand(request.GigId, request.InitialMessage), ct);

        await hub.Clients
            .Group($"user:{result.OtherUserId}")
            .SendAsync("ConversationStarted", result, ct);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyConversations(CancellationToken ct)
    {
        var result = await mediator.Send(new GetMyConversationsQuery(), ct);
        return Ok(result);
    }
    [HttpGet("{id:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 30, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetConversationMessagesQuery(id, page, pageSize), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new SendMessageCommand(id, request.Content), ct);

        await hub.Clients
            .Group($"user:{result.RecipientUserId}")
            .SendAsync("MessageReceived", result.Message, ct);

        return Ok(result.Message);
    }

    [HttpPost("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new MarkConversationReadCommand(id), ct);

        await hub.Clients
            .Group($"user:{result.SenderUserId}")
            .SendAsync("ReadReceipt", result.ConversationId, ct);

        return NoContent();
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var count = await mediator.Send(new GetUnreadCountQuery(), ct);
        return Ok(new { count });
    }
}

public sealed record StartConversationRequest(Guid GigId, string InitialMessage);
public sealed record SendMessageRequest(string Content);