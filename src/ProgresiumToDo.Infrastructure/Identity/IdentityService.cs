using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
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
    }
    
    public async Task<Result<Guid>> RegisterAsync(string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
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
        
        var authTokens = GenerateTokens(user, cancellationToken);
        
        return authTokens;
    }
    
    public AuthenticationResult GenerateTokens(User user, CancellationToken cancellationToken = default)
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

    public async Task<Result<AuthenticationResult>> RefreshTokensAsync(string oldRefreshTokenValue, CancellationToken cancellationToken)
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
        
        var authTokens = GenerateTokens(user, cancellationToken);
        oldRefreshToken.Revoke(authTokens.RefreshToken.Id);
        
        return authTokens;
    }
}