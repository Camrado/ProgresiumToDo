using NSubstitute;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tags.Commands.DeleteTag;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Tags;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Application.Tags.Commands.DeleteTag;

public class DeleteTagCommandValidatorTests
{
    private readonly DeleteTagCommandValidator _validator;
    private readonly ITagRepository _tagRepository;
    private readonly IUserContext _userContext;
    
    public DeleteTagCommandValidatorTests()
    {
        _tagRepository = Substitute.For<ITagRepository>();
        _userContext = Substitute.For<IUserContext>();
        _validator = new DeleteTagCommandValidator(_tagRepository, _userContext);
        
        _userContext.UserId.Returns(Guid.NewGuid());
    }

    [Fact]
    public async Task ValidateAsync_ShouldNotHaveError_WhenCommandIsValid()
    {
        var tagId = Guid.NewGuid();
        var command = new DeleteTagCommand(tagId);
        var userId = _userContext.UserId;
        var tag = Tag.Create("Valid Tag", userId);
        
        _tagRepository
            .GetByIdAndUserIdAsync(tagId, userId, true, Arg.Any<CancellationToken>())
            .Returns(tag);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_ShouldHaveError_WhenTagIdIsEmpty()
    {
        var command = new DeleteTagCommand(Guid.Empty);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(DeleteTagCommand.TagId) && e.ErrorMessage == "TagId must not be empty.");
    }

    [Fact]
    public async Task ValidateAsync_ShouldHaveError_WhenTagDoesNotExist()
    {
        var tagId = Guid.NewGuid();
        var command = new DeleteTagCommand(tagId);
        var userId = _userContext.UserId;
        
        _tagRepository
            .GetByIdAndUserIdAsync(tagId, userId, true, Arg.Any<CancellationToken>())
            .Returns((Tag?)null);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(DeleteTagCommand.TagId) && e.ErrorMessage == "Tag does not exist.");
    }
}
