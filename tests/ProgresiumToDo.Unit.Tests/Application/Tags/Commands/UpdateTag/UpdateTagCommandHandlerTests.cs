using NSubstitute;
using ProgresiumToDo.Application.Tags.Commands.UpdateTag;
using ProgresiumToDo.Domain.Tags;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Application.Tags.Commands.UpdateTag;

public class UpdateTagCommandHandlerTests
{
    private readonly UpdateTagCommandHandler _handler;

    public UpdateTagCommandHandlerTests()
    {
        _handler = new UpdateTagCommandHandler();
    }

    [Fact]
    public async Task Handle_Should_UpdateTag_And_ReturnSuccess()
    {
        var tag = Tag.Create("Old Name", Guid.NewGuid());
        var newName = "New Name";
        
        var command = new UpdateTagCommand(newName)
        {
            TagId = tag.Id,
            Tag = tag
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        tag.Name.ShouldBe(newName);
        result.Value.ShouldBeOfType<UpdateTagCommandResponse>();
    }
}
