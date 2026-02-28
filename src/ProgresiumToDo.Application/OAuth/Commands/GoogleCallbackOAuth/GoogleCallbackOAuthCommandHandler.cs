using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Auth.OAuth;
using ProgresiumToDo.Application.Abstractions.Billing;
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
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<GoogleCallbackOAuthCommandHandler> _logger;
    
    public GoogleCallbackOAuthCommandHandler(
        IMemoryCache memoryCache,
        IOAuthService oAuthService,
        IUserRepository userRepository,
        IIdentityService identityService,
        ISubscriptionService subscriptionService,
        ILogger<GoogleCallbackOAuthCommandHandler> logger)
    {
        _memoryCache = memoryCache;
        _oAuthService = oAuthService;
        _userRepository = userRepository;
        _identityService = identityService;
        _subscriptionService = subscriptionService;
        _logger = logger;
    }
    
    public async Task<Result<GoogleCallbackOAuthCommandResponse>> Handle(GoogleCallbackOAuthCommand request, CancellationToken cancellationToken)
    {
        var saved = _memoryCache.Get<(string, string)?>($"oauth:{request.State}");
        if (saved is null)
        {
            _logger.LogWarning("OAuth callback failed. Invalid or expired state: {State}", request.State);
            return Result.Failure<GoogleCallbackOAuthCommandResponse>([OAuthErrors.InvalidOrExpiredState]);
        }

        var (verifier, nonce) = saved.Value;
        var googleIdentityResult =
            await _oAuthService.GetGoogleIdentityAsync(request.Code, verifier, nonce, cancellationToken);
        
        var user = await _userRepository.GetByEmailAsync(googleIdentityResult.Email, cancellationToken: cancellationToken);
        
        if (user is not null)
        {
            var addGoogleLoginResult = await _identityService.AddGoogleLoginAsync(user.Email, googleIdentityResult.Sub, cancellationToken);
            
            if (addGoogleLoginResult.IsFailure)
            {
                _logger.LogWarning(
                    "OAuth callback failed. Google login linking failed. UserId: {UserId}",
                    user.Id);
                return Result.Failure<GoogleCallbackOAuthCommandResponse>(addGoogleLoginResult.Errors);
            }
            
            _logger.LogInformation("OAuth callback successful. Existing user logged in. UserId: {UserId}", user.Id);
        }
        else
        {
            var createUserResult = await _identityService.RegisterUserAsync(googleIdentityResult.Email);
            if (createUserResult.IsFailure)
            {
                _logger.LogWarning(
                    "OAuth callback failed. User registration failed. Errors: {ErrorCodes}",
                    string.Join(", ", createUserResult.Errors.Select(e => e.Code)));
                return Result.Failure<GoogleCallbackOAuthCommandResponse>(createUserResult.Errors);
            }

            user = User.Create(
                googleIdentityResult.Email, 
                googleIdentityResult.FirstName ?? string.Empty, 
                googleIdentityResult.LastName ?? string.Empty, 
                createUserResult.Value);
            _userRepository.Add(user);

            var addGoogleLoginResult = await _identityService.AddGoogleLoginAsync(user.Email, googleIdentityResult.Sub, cancellationToken);
            
            if (addGoogleLoginResult.IsFailure)
            {
                _logger.LogWarning(
                    "OAuth callback failed. Google login linking failed for new user. UserId: {UserId}",
                    user.Id);
                return Result.Failure<GoogleCallbackOAuthCommandResponse>(addGoogleLoginResult.Errors);
            }
            
            var subscriptionResult = await _subscriptionService.SubscribeUserToFreePlanAsync(user.Id, cancellationToken);
            if (subscriptionResult.IsFailure)
            {
                return Result.Failure<GoogleCallbackOAuthCommandResponse>(subscriptionResult.Errors);
            }
            
            _logger.LogInformation("OAuth callback successful. New user created and logged in. UserId: {UserId}", user.Id);
        }
        
        var tokens = _identityService.GenerateTokens(user);
        
        return new GoogleCallbackOAuthCommandResponse(
            "OAuth login successful.",
            tokens.AccessToken,
            tokens.RefreshToken.Token,
            tokens.ExpiresIn);
    }
}