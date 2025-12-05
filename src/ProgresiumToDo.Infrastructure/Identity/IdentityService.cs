using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ProgresiumToDo.Infrastructure.Identity;

internal sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly string _jwtSecret;
    private readonly int _jwtLifespan;
    private readonly int _refreshTokenLifespan;
    private readonly string _baseUrl;
    
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                     throw new ApplicationException("Jwt secret is missing.");
        _jwtLifespan = int.Parse(Environment.GetEnvironmentVariable("JWT_TOKEN_LIFETIME_IN_SECONDS") ??
                                 throw new ApplicationException("Jwt token lifetime is missing."));
        _refreshTokenLifespan = int.Parse(Environment.GetEnvironmentVariable("REFRESH_TOKEN_LIFETIME_IN_DAYS") ??
                                         throw new ApplicationException("Refresh token lifetime is missing."));
        _baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ??
                   throw new ApplicationException("Base url is missing.");
    }
    
    public async Task<Result<Guid>> RegisterAsync(string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = $"{email}_{Guid.NewGuid()}",
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => new Error(e.Code, e.Description))
                .Where(e => !e.Code.Contains("username", StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            return Result.Failure<Guid>(errors);
        }

        return user.Id;
    }
    
    public async Task<Result<Guid>> CreateUserAsync(string email)
    {
        var user = new ApplicationUser
        {
            UserName = $"{email}_{Guid.NewGuid()}",
            Email = email
        };
        
        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors
                .Select(e => new Error(e.Code, e.Description))
                .Where(e => !e.Code.Contains("username", StringComparison.InvariantCultureIgnoreCase))
                .ToList();
            return Result.Failure<Guid>(errors);
        }

        return user.Id;
    }

    public async Task<Result<AuthenticationResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        if (appUser is null)
        {
            return Result.Failure<AuthenticationResult>([UserErrors.InvalidCredentials]);
        }
        
        var signInResult = await _signInManager.CheckPasswordSignInAsync(appUser, password, false);
        if (!signInResult.Succeeded)
        {
            return Result.Failure<AuthenticationResult>([UserErrors.InvalidCredentials]);
        }
        
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return Result.Failure<AuthenticationResult>([UserErrors.UserNotFound]);
        }
        
        var authTokens = GenerateTokens(user);
        
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
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
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
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure<bool>([UserErrors.UserNotFound]);
        }
        if (user.EmailConfirmed)
        {
            return Result.Failure<bool>([UserErrors.EmailAlreadyVerified]);
        }
        
        var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
        var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            return Result.Failure<bool>([UserErrors.EmailVerificationFailed]);
        }
        
        return Result.Success();
    }

    public async Task<Result<bool>> IsEmailVerifiedAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Failure<bool>([UserErrors.UserNotFound]);
        }
        
        return user.EmailConfirmed;
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
            return Result.Failure<bool>([UserErrors.UserNotFound]);
        }

        await _userManager.DeleteAsync(user);
        
        return Result.Success();
    }
    
    public async Task<Result> AddGoogleLoginAsync(string email, string googleIdentitySub, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Failure<bool>([UserErrors.UserNotFound]);
        }
        
        user.EmailConfirmed = true;

        var loginInfo = new UserLoginInfo("Google", googleIdentitySub, "Google");
        
        var existingLogins = await _userManager.GetLoginsAsync(user);
        if (!existingLogins.Any(userLoginInfo => userLoginInfo.LoginProvider == "Google" && userLoginInfo.ProviderKey == googleIdentitySub))
        {
            var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
            
            if (!addLoginResult.Succeeded)
                return Result.Failure<Result>([OAuthErrors.CannotLinkGoogleAccount]);
        }
        
        return Result.Success();
    }
}