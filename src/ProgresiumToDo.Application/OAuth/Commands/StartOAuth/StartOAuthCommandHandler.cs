using Microsoft.Extensions.Caching.Memory;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Abstractions.OAuth;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.OAuth.Commands.StartOAuth;

internal sealed class StartOAuthCommandHandler : ICommandHandler<StartOAuthCommand, StartOAuthCommandResponse>
{
    private readonly IOAuthService _oAuthService;
    private readonly IMemoryCache _cache;
    
    public StartOAuthCommandHandler(IOAuthService oAuthService, IMemoryCache memoryCache)
    {
        _oAuthService = oAuthService;
        _cache = memoryCache;
    }
    
    public Task<Result<StartOAuthCommandResponse>> Handle(StartOAuthCommand request, CancellationToken cancellationToken)
    {
        var (verifier, challenge) = _oAuthService.GeneratePkce();
        var state = Guid.NewGuid().ToString("N");
        var nonce = Guid.NewGuid().ToString("N");
        
        _cache.Set($"oauth:{state}", (verifier, nonce), TimeSpan.FromMinutes(5));

        var authUrl = _oAuthService.GenerateAuthorizationUrl(request.Provider, challenge, state, nonce);

        var response = new StartOAuthCommandResponse("OAuth flow initiated successfully.", authUrl);
        return Task.FromResult<Result<StartOAuthCommandResponse>>(response);
    }
}