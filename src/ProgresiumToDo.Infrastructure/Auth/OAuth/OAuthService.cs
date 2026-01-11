using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Google.Apis.Auth;
using Microsoft.AspNetCore.WebUtilities;
using ProgresiumToDo.Application.Abstractions.Auth.OAuth;

namespace ProgresiumToDo.Infrastructure.Auth.OAuth;

internal sealed class OAuthService : IOAuthService
{
    private readonly string _googleClientId;
    private readonly string _googleClientSecret;
    private readonly string _baseUrl;
    private readonly IHttpClientFactory _httpClientFactory;
    
    private string RedirectUri => $"{_baseUrl}/api/progresium-todo/v1/oauth/callback/google";
    
    public OAuthService(IHttpClientFactory httpClientFactory)
    {
        _googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ??
                          throw new ApplicationException("Google Client Id is missing.");
        _googleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ??
                          throw new ApplicationException("Google Client Secret is missing.");
        _baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ??
                   throw new ApplicationException("Base url is missing.");
        _httpClientFactory = httpClientFactory;
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

    public async Task<GoogleIdentityResult> GetGoogleIdentityAsync(string code, string verifier, string expectedNonce,
        CancellationToken cancellationToken = default)
    {
        var tokenRequestBody = new Dictionary<string, string>
        {
            ["client_id"] = _googleClientId,
            ["client_secret"] = _googleClientSecret,
            ["code"] = code,
            ["redirect_uri"] = RedirectUri,
            ["grant_type"] = "authorization_code",
            ["code_verifier"] = verifier
        };
        
        var httpClient = _httpClientFactory.CreateClient();

        var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(tokenRequestBody), cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Google token exchange failed");

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var doc = JsonDocument.Parse(json).RootElement;
        
        var idToken = doc.GetProperty("id_token").GetString();
        var validationSettings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [_googleClientId]
        };

        var id = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);
        
        if(!string.Equals(id.Nonce, expectedNonce, StringComparison.Ordinal))
            throw new SecurityException("Nonce mismatch");

        return new GoogleIdentityResult(id.Subject, id.Email, id.GivenName, id.FamilyName);
    }
}