namespace ProgresiumToDo.Application.Abstractions.Behaviors;

public interface IApplicationTransaction : IDisposable, IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    Task RollbackAsync(CancellationToken cancellationToken = default);
}