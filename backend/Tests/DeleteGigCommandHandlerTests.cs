using GigMarket.Application.Common.Interfaces;
using GigMarket.Application.Features.Gigs.Commands.DeleteGig;
using MediatR;
using NSubstitute;

namespace Tests;

public class DeleteGigCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Call_GigService_DeleteGigAsync()
    {
        var gigService = Substitute.For<IGigService>();
        var handler = new DeleteGigCommandHandler(gigService);

        var gigId = Guid.NewGuid();
        var command = new DeleteGigCommand(gigId);

        await handler.Handle(command, CancellationToken.None);

        await gigService
            .Received(1)
            .DeleteGigAsync(gigId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Return_Unit_Value()
    {
        var gigService = Substitute.For<IGigService>();
        var handler = new DeleteGigCommandHandler(gigService);

        var command = new DeleteGigCommand(Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
    }
}