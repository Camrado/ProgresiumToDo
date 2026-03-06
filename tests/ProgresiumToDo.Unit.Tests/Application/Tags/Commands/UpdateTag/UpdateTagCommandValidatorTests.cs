using NSubstitute;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tags.Commands.UpdateTag;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Tags;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Application.Tags.Commands.UpdateTag;

public class UpdateTagCommandValidatorTests
{
    private readonly UpdateTagCommandValidator _validator;
    private readonly ITagRepository _tagRepository;
    private readonly IUserContext _userContext;
    
    public UpdateTagCommandValidatorTests()
    {
        _tagRepository = Substitute.For<ITagRepository>();
        _userContext = Substitute.For<IUserContext>();
        _validator = new UpdateTagCommandValidator(_tagRepository, _userContext);
        
        _userContext.UserId.Returns(Guid.NewGuid());
    }

    [Fact]
    public async Task ValidateAsync_ShouldNotHaveError_WhenCommandIsValid()
    {
        var tagId = Guid.NewGuid();
        var command = new UpdateTagCommand("Valid New Name") { TagId = tagId };
        var userId = _userContext.UserId;
        var existingTag = Tag.Create("Old Name", userId);
        
        _tagRepository
            .GetByIdAndUserIdAsync(tagId, userId, true, Arg.Any<CancellationToken>())
            .Returns(existingTag);
            
        _tagRepository
            .GetByNameAsync(command.Name, userId, false, Arg.Any<CancellationToken>())
            .Returns((Tag?)null);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_ShouldNotHaveError_WhenNameIsUnchanged()
    {
        var tagId = Guid.NewGuid();
        var sameName = "Same Name";
        var command = new UpdateTagCommand(sameName) { TagId = tagId };
        var userId = _userContext.UserId;
        var existingTag = Tag.Create(sameName, userId);
        
        _tagRepository
            .GetByIdAndUserIdAsync(tagId, userId, true, Arg.Any<CancellationToken>())
            .Returns(existingTag);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public async Task ValidateAsync_ShouldHaveError_WhenTagIdIsEmpty()
    {
        var command = new UpdateTagCommand("New Name") { TagId = Guid.Empty };

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(UpdateTagCommand.TagId) && e.ErrorMessage == "TagId must not be empty.");
    }

    [Fact]
    public async Task ValidateAsync_ShouldHaveError_WhenTagDoesNotExist()
    {
        var tagId = Guid.NewGuid();
        var command = new UpdateTagCommand("New Name") { TagId = tagId };
        var userId = _userContext.UserId;
        
        _tagRepository
            .GetByIdAndUserIdAsync(tagId, userId, true, Arg.Any<CancellationToken>())
            .Returns((Tag?)null);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(UpdateTagCommand.TagId) && e.ErrorMessage == "Tag does not exist.");
    }

    [Fact]
    public async Task ValidateAsync_ShouldHaveError_WhenNameExceedsLength()
    {
        var tagId = Guid.NewGuid();
        var longName = new string('A', 256);
        var command = new UpdateTagCommand(longName) { TagId = tagId };
        var userId = _userContext.UserId;
        var existingTag = Tag.Create("Old Name", userId);
        
        _tagRepository
            .GetByIdAndUserIdAsync(tagId, userId, true, Arg.Any<CancellationToken>())
            .Returns(existingTag);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(UpdateTagCommand.Name) && e.ErrorMessage == "Tag name must not exceed 255 characters.");
    }

    [Fact]
    public async Task ValidateAsync_ShouldHaveError_WhenNameAlreadyExists()
    {
        var tagId = Guid.NewGuid();
        var existingName = "Already Taken";
        var command = new UpdateTagCommand(existingName) { TagId = tagId };
        var userId = _userContext.UserId;
        var currentTag = Tag.Create("Old Name", userId);
        var conflictingTag = Tag.Create(existingName, userId);
        
        _tagRepository
            .GetByIdAndUserIdAsync(tagId, userId, true, Arg.Any<CancellationToken>())
            .Returns(currentTag);
            
        _tagRepository
            .GetByNameAsync(command.Name!, userId, false, Arg.Any<CancellationToken>())
            .Returns(conflictingTag);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(UpdateTagCommand.Name) && e.ErrorMessage == "A tag with the same name already exists.");
    }
}
