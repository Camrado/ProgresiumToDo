using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.Commands.RefreshTokens;

public record RefreshTokensCommand(string OldRefreshToken) : ICommand<RefreshTokensCommandResponse>;