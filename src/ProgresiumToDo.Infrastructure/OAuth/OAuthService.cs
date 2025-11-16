using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using ProgresiumToDo.Application.Abstractions.OAuth;

namespace ProgresiumToDo.Infrastructure.OAuth;

internal sealed class OAuthService : IOAuthService
{
    private readonly string _googleClientId;
    private readonly string _baseUrl;
    
    private string RedirectUri => $"{_baseUrl}/api/progresium-todo/v1/oauth/callback/google";
    
    public OAuthService()
    {
        _googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ??
                          throw new ApplicationException("Google Client Id is missing.");
        _baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ??
                   throw new ApplicationException("Base url is missing.");
    }
    
    public (string verifier, string challenge) GeneratePkce()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[64];
        rng.GetBytes(bytes);
        var verifier = WebEncoders.Base64UrlEncode(bytes);
        
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(verifier));
        var challenge = WebEncoders.Base64UrlEncode(hash);
        return (verifier, challenge);
    }

    public string GenerateAuthorizationUrl(string provider, string challenge, string state, string nonce)
    {
        var authUrl = string.Empty;
        
        if (string.Equals(provider, "google", StringComparison.OrdinalIgnoreCase))
        {
            authUrl = QueryHelpers.AddQueryString(
                "https://accounts.google.com/o/oauth2/v2/auth",
                new Dictionary<string, string?>
                {
                    ["client_id"] = _googleClientId,
                    ["redirect_uri"] = RedirectUri,
                    ["response_type"] = "code",
                    ["scope"] = "openid email profile",
                    ["state"] = state,
                    ["nonce"] = nonce,
                    ["code_challenge"] = challenge,
                    ["code_challenge_method"] = "S256"
                });   
        }
        
        return authUrl;
    }
}