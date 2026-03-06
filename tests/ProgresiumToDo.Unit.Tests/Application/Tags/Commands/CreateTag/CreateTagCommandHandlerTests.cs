using NSubstitute;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tags.Commands.CreateTag;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Tags;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Application.Tags.Commands.CreateTag;

public class CreateTagCommandHandlerTests
{
    private readonly CreateTagCommandHandler _handler;
    private readonly ITagRepository _tagRepository;
    private readonly IUserContext _userContext;
    
    public CreateTagCommandHandlerTests()
    {
        _tagRepository = Substitute.For<ITagRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new CreateTagCommandHandler(_tagRepository, _userContext);

        _userContext.UserId.Returns(Guid.NewGuid());
    }

    [Theory]
    [InlineData("Valid Name")]
    [InlineData("Another Valid Name")]
    public async Task Handle_Should_Pass(string tagName)
    {
        var command = new CreateTagCommand(tagName);

        var result = await _handler.Handle(command, CancellationToken.None);
        
        _tagRepository.Received(1).Add(Arg.Is<Tag>(t => t.Name == tagName));
        result.Value.ShouldBeOfType<CreateTagCommandResponse>();
        result.Value.Tag.Name.ShouldBe(tagName);
    }
}