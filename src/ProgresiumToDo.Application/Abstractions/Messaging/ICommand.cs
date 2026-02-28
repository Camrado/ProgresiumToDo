using MediatR;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface IBaseCommand;

/// <summary>
/// Marker interface for commands that opt out of automatic UoW transaction wrapping.
/// Handlers for these commands are responsible for manually managing transactions and calling SaveChangesAsync.
/// Use this when a command includes external I/O (HTTP calls, email sends) that should not hold a DB transaction open.
/// </summary>
public interface INonTransactionalCommand;