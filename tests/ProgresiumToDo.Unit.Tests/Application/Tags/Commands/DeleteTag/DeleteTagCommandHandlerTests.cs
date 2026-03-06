using NSubstitute;
using ProgresiumToDo.Application.Tags.Commands.DeleteTag;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Tags;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Application.Tags.Commands.DeleteTag;

public class DeleteTagCommandHandlerTests
{
    private readonly ITagRepository _tagRepository;
    private readonly DeleteTagCommandHandler _handler;
    
    public DeleteTagCommandHandlerTests()
    {
        _tagRepository = Substitute.For<ITagRepository>();
        _handler = new DeleteTagCommandHandler(_tagRepository);
    }
    
    [Fact]
    public async Task Handle_Should_CallDeleteAndReturnSuccess()
    {
        var tag = Tag.Create("Any Tag", Guid.NewGuid());
        var command = new DeleteTagCommand(tag.Id)
        {
            Tag = tag
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        _tagRepository.Received(1).Delete(tag);
        result.Value.ShouldBeOfType<DeleteTagCommandResponse>();
    }
}
