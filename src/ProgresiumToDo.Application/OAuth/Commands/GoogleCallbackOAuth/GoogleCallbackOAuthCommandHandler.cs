using Microsoft.Extensions.Caching.Memory;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Auth.OAuth;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;
using ProgresiumToDo.Domain.Auth.Errors;

namespace ProgresiumToDo.Application.OAuth.Commands.GoogleCallbackOAuth;

internal sealed class GoogleCallbackOAuthCommandHandler : ICommandHandler<GoogleCallbackOAuthCommand, GoogleCallbackOAuthCommandResponse>
{
    private readonly IMemoryCache _memoryCache;
    private readonly IOAuthService _oAuthService;
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    
    public GoogleCallbackOAuthCommandHandler(
        IMemoryCache memoryCache,
        IOAuthService oAuthService,
        IUserRepository userRepository,
        IIdentityService identityService)
    {
        _memoryCache = memoryCache;
        _oAuthService = oAuthService;
        _userRepository = userRepository;
        _identityService = identityService;
    }
    
    public async Task<Result<GoogleCallbackOAuthCommandResponse>> Handle(GoogleCallbackOAuthCommand request, CancellationToken cancellationToken)
    {
        var saved = _memoryCache.Get<(string, string)?>($"oauth:{request.State}");
        if (saved is null)
            return Result.Failure<GoogleCallbackOAuthCommandResponse>([OAuthErrors.InvalidOrExpiredState]);

        var (verifier, nonce) = saved.Value;
        var googleIdentityResult =
            await _oAuthService.GetGoogleIdentityAsync(request.Code, verifier, nonce, cancellationToken);
        
        var user = await _userRepository.GetByEmailAsync(googleIdentityResult.Email, cancellationToken);
        
        if (user is not null)
        {
            var addGoogleLoginResult = await _identityService.AddGoogleLoginAsync(user.Email, googleIdentityResult.Sub, cancellationToken);
            
            if (addGoogleLoginResult.IsFailure)
                return Result.Failure<GoogleCallbackOAuthCommandResponse>(addGoogleLoginResult.Errors);
        }
        else
        {
            var createUserResult = await _identityService.RegisterUserAsync(googleIdentityResult.Email);
            if (createUserResult.IsFailure)
                return Result.Failure<GoogleCallbackOAuthCommandResponse>(createUserResult.Errors);

            user = User.Create(googleIdentityResult.Email, googleIdentityResult.FirstName, googleIdentityResult.LastName, createUserResult.Value);
            _userRepository.Add(user);

            var addGoogleLoginResult = await _identityService.AddGoogleLoginAsync(user.Email, googleIdentityResult.Sub, cancellationToken);
            
            if (addGoogleLoginResult.IsFailure)
                return Result.Failure<GoogleCallbackOAuthCommandResponse>(addGoogleLoginResult.Errors);
        }
        
        var tokens = _identityService.GenerateTokens(user);
        
        return new GoogleCallbackOAuthCommandResponse(
            "OAuth login successful.",
            tokens.AccessToken,
            tokens.RefreshToken.Token,
            tokens.ExpiresIn);
    }
}