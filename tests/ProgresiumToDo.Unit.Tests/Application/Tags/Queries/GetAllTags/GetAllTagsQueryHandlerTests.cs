using NSubstitute;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tags.Queries.GetAllTags;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Tags;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Application.Tags.Queries.GetAllTags;

public class GetAllTagsQueryHandlerTests
{
    private readonly ITagRepository _tagRepository;
    private readonly IUserContext _userContext;
    private readonly GetAllTagsQueryHandler _handler;

    public GetAllTagsQueryHandlerTests()
    {
        _tagRepository = Substitute.For<ITagRepository>();
        _userContext = Substitute.For<IUserContext>();
        _handler = new GetAllTagsQueryHandler(_tagRepository, _userContext);

        _userContext.UserId.Returns(Guid.NewGuid());
    }

    [Fact]
    public async Task Handle_Should_ReturnAllTagsForUser()
    {
        var tag1 = Tag.Create("Tag 1", _userContext.UserId);
        var tag2 = Tag.Create("Tag 2", _userContext.UserId);
        
        _tagRepository
            .GetAllAsync(_userContext.UserId, false, Arg.Any<CancellationToken>())
            .Returns([tag1, tag2]);

        var query = new GetAllTagsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Value.ShouldBeOfType<GetAllTagsQueryResponse>();
        result.Value.Tags.Count().ShouldBe(2);
        result.Value.Tags.ShouldContain(t => t.Id == tag1.Id && t.Name == tag1.Name);
        result.Value.Tags.ShouldContain(t => t.Id == tag2.Id && t.Name == tag2.Name);
    }
}
