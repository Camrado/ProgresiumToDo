using MediatR;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;