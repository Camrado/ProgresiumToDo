using System.Diagnostics;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Google.Apis.Auth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ProgresiumToDo.Application.Abstractions.Auth.OAuth;

namespace ProgresiumToDo.Infrastructure.Auth.OAuth;

internal sealed class OAuthService : IOAuthService
{
    private readonly string _googleClientId;
    private readonly string _googleClientSecret;
    private readonly string _baseUrl;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OAuthService> _logger;
    
    private string RedirectUri => $"{_baseUrl}/api/progresium-todo/v1/oauth/callback/google";
    
    public OAuthService(IHttpClientFactory httpClientFactory, ILogger<OAuthService> logger)
    {
        _googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") ??
                          throw new ApplicationException("Google Client Id is missing.");
        _googleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET") ??
                          throw new ApplicationException("Google Client Secret is missing.");
        _baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ??
                   throw new ApplicationException("Base url is missing.");
        _httpClientFactory = httpClientFactory;
        _logger = logger;
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
        else
        {
            _logger.LogWarning("Unsupported OAuth provider requested: {Provider}", provider);
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

        _logger.LogInformation("Initiating Google OAuth token exchange");
        var stopwatch = Stopwatch.StartNew();
        
        var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(tokenRequestBody), cancellationToken);
        
        stopwatch.Stop();
        _logger.LogInformation(
            "Google OAuth token exchange completed. StatusCode: {StatusCode}, Duration: {DurationMs}ms",
            (int)response.StatusCode,
            stopwatch.ElapsedMilliseconds);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(
                "Google token exchange failed. StatusCode: {StatusCode}, Duration: {DurationMs}ms. ErrorContent: {ErrorContent}",
                (int)response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                errorContent);
            
            throw new InvalidOperationException("Google token exchange failed");
        }

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        
        try 
        {
            var doc = JsonDocument.Parse(json).RootElement;
            var idToken = doc.GetProperty("id_token").GetString();
        
            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [_googleClientId]
            };

            var id = await GoogleJsonWebSignature.ValidateAsync(idToken, validationSettings);
        
            if (!string.Equals(id.Nonce, expectedNonce, StringComparison.Ordinal))
            {
                _logger.LogWarning(
                    "OAuth nonce mismatch. Expected: {ExpectedNonce}, Actual: {ActualNonce} (Subject: {Subject})", 
                    expectedNonce, id.Nonce, id.Subject);
                throw new SecurityException("Nonce mismatch");
            }

            return new GoogleIdentityResult(id.Subject, id.Email, id.GivenName, id.FamilyName);
        }
        catch (Exception ex) when (ex is not SecurityException)
        {
            _logger.LogError(ex, "Failed to parse or validate Google ID token");
            throw;
        }
    }
}