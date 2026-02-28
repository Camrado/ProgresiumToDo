using Hangfire.Logging;
using Hangfire.Server;
using ProgresiumToDo.Application.Abstractions.BackgroundJobs;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Infrastructure.Services.BackgroundJobs;

internal sealed class ResultCheckJobFilter : IServerFilter
{
    private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

    public void OnPerforming(PerformingContext context)
    {
    }

    public void OnPerformed(PerformedContext context)
    {
        if (context.Result is Result { IsFailure: true } result)
        {
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Message}"));

            Logger.WarnFormat(
                "Background job {0} ({1}.{2}) returned Result.Failure: {3}",
                context.BackgroundJob.Id,
                context.BackgroundJob.Job.Type.Name,
                context.BackgroundJob.Job.Method.Name,
                errors);

            throw new BackgroundJobFailedException(result.Errors);
        }
    }
}
