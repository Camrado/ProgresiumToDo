using FluentValidation;
using ProgresiumToDo.Application.Tags.Repositories;

namespace ProgresiumToDo.Application.Tags.Queries.GetSingleTag;

internal sealed class GetSingleTagQueryValidator : AbstractValidator<GetSingleTagQuery>
{
    public GetSingleTagQueryValidator(ITagRepository tagRepository)
    {
        RuleFor(gst => gst.TagId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("TagId must not be empty.")
            .MustAsync(async (query, tagId, cancellationToken) =>
            {
                var tag = await tagRepository.GetByIdAsync(tagId, cancellationToken);
                query.Tag = tag;
                
                return tag is not null;
            })
            .WithMessage("Tag does not exist.");
    }
}