using NSubstitute;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Tags.Commands.CreateTag;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Tags;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Application.Tags.Commands.CreateTag;

public class CreateTagCommandValidatorTests
{
    private readonly CreateTagCommandValidator _validator;
    private readonly ITagRepository _tagRepository;
    private readonly IUserContext _userContext;
    
    public CreateTagCommandValidatorTests()
    {
        _tagRepository = Substitute.For<ITagRepository>();
        _userContext = Substitute.For<IUserContext>();
        _validator = new CreateTagCommandValidator(_tagRepository, _userContext);

        _userContext.UserId.Returns(Guid.NewGuid());
    }

    [Fact]
    public async Task ValidateAsync_ShouldNotHaveError_WhenCommandIsValid()
    {
        var command = new CreateTagCommand("Valid Name");
        _tagRepository.GetByNameAsync(command.Name, _userContext.UserId, false, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Tag?>(null));

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_ShouldHaveError_WhenNameIsEmpty(string? name)
    {
        var command = new CreateTagCommand(name!);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateTagCommand.Name) && e.ErrorMessage == "Tag name is required.");
    }

    [Fact]
    public async Task ValidateAsync_ShouldHaveError_WhenNameExceeds255Characters()
    {
        var longName = new string('A', 256);
        var command = new CreateTagCommand(longName);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateTagCommand.Name) && e.ErrorMessage == "Tag name must not exceed 255 characters.");
    }

    [Fact]
    public async Task ValidateAsync_ShouldHaveError_WhenTagAlreadyExists()
    {
        var command = new CreateTagCommand("ExistingTag");
        var existingTag = Tag.Create(command.Name, _userContext.UserId);
        
        _tagRepository.GetByNameAsync(command.Name, _userContext.UserId, false, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Tag?>(existingTag));

        var result = await _validator.ValidateAsync(command);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == nameof(CreateTagCommand.Name) && e.ErrorMessage == "A tag with the same name already exists.");
    }
}