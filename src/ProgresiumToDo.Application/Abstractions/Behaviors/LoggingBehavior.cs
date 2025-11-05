using MediatR;
using Microsoft.Extensions.Logging;

namespace ProgresiumToDo.Application.Abstractions.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = request.GetType().Name;

        try
        {
            _logger.LogInformation("Handling request: {Request}", requestName);

            var response = await next(cancellationToken);

            _logger.LogInformation("Request handled successfully: {Request}", requestName);

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error handling request: {Request}", requestName);
            throw;
        }
    }
}