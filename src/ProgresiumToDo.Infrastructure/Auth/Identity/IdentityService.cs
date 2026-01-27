using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Auth.Repositories;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;
using ProgresiumToDo.Domain.Auth.Errors;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ProgresiumToDo.Infrastructure.Auth.Identity;

internal sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<IdentityService> _logger;
    private readonly string _jwtSecret;
    private readonly int _jwtLifespan;
    private readonly int _refreshTokenLifespan;
    private readonly string _baseUrl;
    
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ILogger<IdentityService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _logger = logger;
        _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                     throw new ApplicationException("Jwt secret is missing.");
        _jwtLifespan = int.Parse(Environment.GetEnvironmentVariable("JWT_TOKEN_LIFETIME_IN_SECONDS") ??
                                 throw new ApplicationException("Jwt token lifetime is missing."));
        _refreshTokenLifespan = int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_LIFETIME_IN_DAYS") ??
                                         throw new ApplicationException("Refresh token lifetime is missing."));
        _baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ??
                   throw new ApplicationException("Base url is missing.");
    }
    
    public async Task<Result<Guid>> RegisterUserAsync(string email, string? password = null)
    {
        var user = new ApplicationUser
        {
            UserName = $"{email}_{Guid.NewGuid()}",
            Email = email
        };

        var result = password is not null ?
            await _userManager.CreateAsync(user, password) :
            await _userManager.CreateAsync(user);
        
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => new Error(e.Code, e.Description))
                .Where(e => !e.Code.Contains("username", StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            
            _logger.LogWarning(
                "User registration failed. Errors: {ErrorCodes}",
                string.Join(", ", errors.Select(e => e.Code)));
            
            return Result.Failure<Guid>(errors);
        }

        _logger.LogInformation("User registered successfully. UserId: {UserId}", user.Id);
        return user.Id;
    }

    public async Task<Result<AuthenticationResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser is null)
        {
            _logger.LogInformation("Login failed. User not found");
            return Result.Failure<AuthenticationResult>([UserErrors.InvalidCredentials]);
        }
        
        var signInResult = await _signInManager.CheckPasswordSignInAsync(appUser, password, false);
        if (!signInResult.Succeeded)
        {
            _logger.LogInformation("Login failed. Invalid credentials. UserId: {UserId}", appUser.Id);
            return Result.Failure<AuthenticationResult>([UserErrors.InvalidCredentials]);
        }
        
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Login failed. User not found in repository. UserId: {UserId}", appUser.Id);
            return Result.Failure<AuthenticationResult>([UserErrors.UserNotFound]);
        }
        
        var authTokens = GenerateTokens(user);
        
        _logger.LogInformation("User logged in successfully. UserId: {UserId}", user.Id);
        return authTokens;
    }
    
    public AuthenticationResult GenerateTokens(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddSeconds(_jwtLifespan);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(nameof(CustomClaim.EmailVerified), user.IsEmailVerified.ToString())
        };

        var jwt = new JwtSecurityToken(
            issuer: _configuration["Authentication:Issuer"],
            audience: _configuration["Authentication:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        var refreshToken = RefreshToken.Create(user.Id, DateTime.UtcNow.AddDays(_refreshTokenLifespan));
        _refreshTokenRepository.Add(refreshToken);
        
        return new AuthenticationResult(accessToken, refreshToken, _jwtLifespan);
    }

    public async Task<Result<AuthenticationResult>> RefreshTokensAsync(string oldRefreshTokenValue, CancellationToken cancellationToken = default)
    {
        var oldRefreshToken = await _refreshTokenRepository.GetByTokenAsync(oldRefreshTokenValue, cancellationToken);
        if (oldRefreshToken is null || !oldRefreshToken.IsActive)
        {
            return Result.Failure<AuthenticationResult>([RefreshTokenErrors.InvalidToken]);
        }
        
        var user = await _userRepository.GetByIdAsync(oldRefreshToken.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<AuthenticationResult>([UserErrors.UserNotFound]);
        }
        
        var authTokens = GenerateTokens(user);
        oldRefreshToken.Revoke(authTokens.RefreshToken.Id);
        
        return authTokens;
    }

    public async Task<Result> VerifyEmailAsync(string userId, string token)
    {
        var appUser = await _userManager.FindByIdAsync(userId);
        if (appUser is null)
        {
            _logger.LogWarning("Email verification failed. User not found. UserId: {UserId}", userId);
            return Result.Failure<bool>([UserErrors.UserNotFound]);
        }
        if (appUser.EmailConfirmed)
        {
            _logger.LogWarning("Email verification failed. Email already verified. UserId: {UserId}", userId);
            return Result.Failure<bool>([UserErrors.EmailAlreadyVerified]);
        }
        
        var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await _userManager.ConfirmEmailAsync(appUser, decodedToken);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Email verification failed. UserId: {UserId}", userId);
            return Result.Failure<bool>([UserErrors.EmailVerificationFailed]);
        }

        var user = await _userRepository.GetByEmailAsync(appUser.Email!);
        if (user is null)
        {
            _logger.LogWarning("Email verification failed. User not found in repository. UserId: {UserId}", userId);
            return Result.Failure<bool>([UserErrors.UserNotFound]);
        }
        
        user.VerifyEmail();
        
        _logger.LogInformation("Email verified successfully. UserId: {UserId}", userId);
        return Result.Success();
    }
    
    public async Task<Result<string>> GenerateEmailVerificationUrlAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Failure<string>([UserErrors.UserNotFound]);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var encodedToken = WebEncoders.Base64UrlEncode(tokenBytes);
        
        var verificationUrl = $"{_baseUrl}/api/progresium-todo/v1/auth/verify-email?userId={user.Id}&verificationToken={encodedToken}";
        
        return verificationUrl;
    }

    public async Task<Result> DeleteAccountAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            _logger.LogWarning("Account deletion failed. User not found");
            return Result.Failure<bool>([UserErrors.UserNotFound]);
        }

        await _userManager.DeleteAsync(user);
        
        _logger.LogInformation("Account deleted successfully. UserId: {UserId}", user.Id);
        return Result.Success();
    }
    
    public async Task<Result> AddGoogleLoginAsync(string email, string googleIdentitySub, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            _logger.LogWarning("Google login linking failed. User not found");
            return Result.Failure<bool>([UserErrors.UserNotFound]);
        }
        
        user.EmailConfirmed = true;

        var loginInfo = new UserLoginInfo("Google", googleIdentitySub, "Google");
        
        var existingLogins = await _userManager.GetLoginsAsync(user);
        if (!existingLogins.Any(userLoginInfo => userLoginInfo.LoginProvider == "Google" && userLoginInfo.ProviderKey == googleIdentitySub))
        {
            var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
            
            if (!addLoginResult.Succeeded)
            {
                _logger.LogWarning("Google login linking failed. UserId: {UserId}", user.Id);
                return Result.Failure<Result>([OAuthErrors.CannotLinkGoogleAccount]);
            }
            
            _logger.LogInformation("Google login linked successfully (new link). UserId: {UserId}", user.Id);
        }
        else
        {
            _logger.LogInformation("Google login already linked. UserId: {UserId}", user.Id);
        }
        
        return Result.Success();
    }
}