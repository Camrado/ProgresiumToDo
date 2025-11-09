using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Auth.RefreshTokens;

public record RefreshTokensCommand(string OldRefreshToken) : ICommand<RefreshTokensCommandResponse>;