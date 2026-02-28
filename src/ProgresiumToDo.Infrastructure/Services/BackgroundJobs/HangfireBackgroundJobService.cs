using System.Linq.Expressions;
using Hangfire;
using ProgresiumToDo.Application.Abstractions.BackgroundJobs;

namespace ProgresiumToDo.Infrastructure.Services.BackgroundJobs;

internal sealed class HangfireBackgroundJobService : IBackgroundJobService
{
    public string EnqueueFireAndForgetJob<T>(Expression<Func<T, Task>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }
}
