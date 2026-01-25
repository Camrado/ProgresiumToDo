using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Tags.Errors;

public static class TagErrors
{
    public static Error NotFound(List<Guid> missingTagIds) => new(
        "Tag.NotFound",
        $"The following tags were not found: {string.Join(", ", missingTagIds)}");
    
    public static Error NotInProject(List<Guid> invalidTagIds, Guid projectId) => new(
        "Tag.NotInProject",
        $"The following tags do not belong to project {projectId}: {string.Join(", ", invalidTagIds)}");
    
    public static Error ProjectIdRequiredForTags => new(
        "Tag.ProjectIdRequiredForTags",
        "ProjectId is required when associating tags with a task.");
}