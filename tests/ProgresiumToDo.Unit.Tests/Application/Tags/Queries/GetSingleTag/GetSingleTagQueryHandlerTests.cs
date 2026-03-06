using NSubstitute;
using ProgresiumToDo.Application.Tags.Queries.GetSingleTag;
using ProgresiumToDo.Domain.Tags;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Application.Tags.Queries.GetSingleTag;

public class GetSingleTagQueryHandlerTests
{
    private readonly GetSingleTagQueryHandler _handler;

    public GetSingleTagQueryHandlerTests()
    {
        _handler = new GetSingleTagQueryHandler();
    }

    [Fact]
    public async Task Handle_Should_ReturnTagDto()
    {
        var tag = Tag.Create("Single Tag", Guid.NewGuid());
        var query = new GetSingleTagQuery(tag.Id)
        {
            Tag = tag
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Value.ShouldBeOfType<GetSingleTagQueryResponse>();
        result.Value.Tag.Id.ShouldBe(tag.Id);
        result.Value.Tag.Name.ShouldBe(tag.Name);
    }
}
