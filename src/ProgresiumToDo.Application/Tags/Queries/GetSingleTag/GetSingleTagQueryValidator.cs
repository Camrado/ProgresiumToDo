using FluentValidation;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Projects.Repositories;
using ProgresiumToDo.Application.Tags.Repositories;

namespace ProgresiumToDo.Application.Tags.Queries.GetSingleTag;

internal sealed class GetSingleTagQueryValidator : AbstractValidator<GetSingleTagQuery>
{
    public GetSingleTagQueryValidator(IProjectRepository projectRepository, ITagRepository tagRepository, IUserContext userContext)
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(gst => gst.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId must not be empty.")
            .MustAsync(async (query, projectId, cancellationToken) =>
            {
                var project = await projectRepository.GetByIdAndUserIdAsync(projectId, userContext.UserId, cancellationToken);
                query.Project = project;
                
                return project is not null;
            })
            .WithMessage("Project does not exist.");
        
        RuleFor(gst => gst.TagId)
            .NotEmpty()
            .WithMessage("TagId must not be empty.")
            .MustAsync(async (query, tagId, cancellationToken) =>
            {
                var tag = await tagRepository.GetByIdAndProjectIdAsync(tagId, query.ProjectId, cancellationToken);
                query.Tag = tag;
                
                return tag is not null;
            })
            .WithMessage("Tag does not exist.");
    }
}