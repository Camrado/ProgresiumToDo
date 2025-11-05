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
    private readonly string _jwtSecret;
    private readonly int _jwtLifespan;
    
    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                     throw new ApplicationException("Jwt secret is missing.");
        _jwtLifespan = int.Parse(Environment.GetEnvironmentVariable("JWT_TOKEN_LIFETIME") ??
                                 throw new ApplicationException("Jwt token lifetime is missing."));
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

    public Task<Result<AuthenticationResult>> LoginAsync(string email, string password)
    {
        throw new NotImplementedException();
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
        
        // TODO: Implement refresh token generation and storage
        var refreshToken = Guid.NewGuid().ToString();
        
        return new AuthenticationResult(accessToken, refreshToken, _jwtLifespan);
    }
}