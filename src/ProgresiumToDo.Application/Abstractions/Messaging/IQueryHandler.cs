using MediatR;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>> 
    where TQuery : IQuery<TResponse>;