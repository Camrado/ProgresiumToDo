using MediatR;
using Microsoft.Extensions.Logging;
using ProgresiumToDo.Application.Abstractions.Behaviors.Contracts;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.Behaviors;

public sealed class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;
    
    public UnitOfWorkBehavior(IUnitOfWork unitOfWork, ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IBaseQuery)
        {
            return await next(cancellationToken);
        }
        
        var requestName = request.GetType().Name;
        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var response = await next(cancellationToken);
            
            if (IsFailureResult(response))
            {
                _logger.LogWarning(
                    "Transaction rollback initiated due to failure result. Request: {RequestName}",
                    requestName);
                
                await transaction.RollbackAsync(cancellationToken);
                return response;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            
            _logger.LogInformation(
                "Transaction committed successfully. Request: {RequestName}",
                requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Transaction rollback initiated due to exception. Request: {RequestName}",
                requestName);
            
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
    
    private static bool IsFailureResult(TResponse response)
    {
        return response is Result { IsFailure: true };
    }
}