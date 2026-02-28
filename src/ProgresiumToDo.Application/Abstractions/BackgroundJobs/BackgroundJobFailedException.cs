using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.BackgroundJobs;

public sealed class BackgroundJobFailedException : Exception
{
    public List<Error> Errors { get; }

    public BackgroundJobFailedException(List<Error> errors)
        : base($"Background job failed: {string.Join("; ", errors.Select(e => $"{e.Code}: {e.Message}"))}")
    {
        Errors = errors;
    }
}
