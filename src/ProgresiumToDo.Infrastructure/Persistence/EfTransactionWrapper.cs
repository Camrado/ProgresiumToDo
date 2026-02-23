using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using ProgresiumToDo.Application.Abstractions.Behaviors.Contracts;

namespace ProgresiumToDo.Infrastructure.Persistence;

internal sealed class EfTransactionWrapper : IApplicationTransaction
{
    private readonly IDbContextTransaction _transaction;
    
    public EfTransactionWrapper(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }
    
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.CommitAsync(cancellationToken);
    }
    
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        await _transaction.RollbackAsync(cancellationToken);
    }
    
    public void Dispose()
    {
        _transaction.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();
    }
}