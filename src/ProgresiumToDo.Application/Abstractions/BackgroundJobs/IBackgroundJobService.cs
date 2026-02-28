using System.Linq.Expressions;

namespace ProgresiumToDo.Application.Abstractions.BackgroundJobs;

public interface IBackgroundJobService
{
    string EnqueueFireAndForgetJob<T>(Expression<Func<T, Task>> methodCall);
}
