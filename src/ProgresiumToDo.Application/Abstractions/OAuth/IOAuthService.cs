namespace ProgresiumToDo.Application.Abstractions.OAuth;

public interface IOAuthService
{
    (string verifier, string challenge) GeneratePkce();
    
    string GenerateAuthorizationUrl(string provider, string challenge, string state, string nonce);

    Task<GoogleIdentityResult> GetGoogleIdentityAsync(string code, string verifier, string expectedNonce,
        CancellationToken cancellationToken = default);
}