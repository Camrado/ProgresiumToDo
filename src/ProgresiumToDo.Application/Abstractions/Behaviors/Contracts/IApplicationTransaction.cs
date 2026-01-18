namespace ProgresiumToDo.Application.Abstractions.Behaviors.Contracts;

public interface IApplicationTransaction : IDisposable, IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    Task RollbackAsync(CancellationToken cancellationToken = default);
}