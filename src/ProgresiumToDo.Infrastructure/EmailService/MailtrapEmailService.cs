using System.Net.Http.Json;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Infrastructure.EmailService;

internal sealed class MailtrapEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly MailtrapSettings _mailtrapSettings;
    private readonly ILogger<MailtrapEmailService> _logger;
    
    public MailtrapEmailService(HttpClient httpClient, IOptions<MailtrapSettings> mailtrapOptions, ILogger<MailtrapEmailService> logger)
    {
        _httpClient = httpClient;
        _mailtrapSettings = mailtrapOptions.Value;
        _logger = logger;
    }
    
    public async Task<Result> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailRequest = new
            {
                from = new
                {
                    email = _mailtrapSettings.SenderEmail,
                    name = _mailtrapSettings.SenderName
                },
                to = new[]
                {
                    new { email = to }
                },
                subject = subject,
                html = body,
                category = "Email Verification"
            };

            var response = await _httpClient.PostAsJsonAsync("send", emailRequest, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Mailtrap Error: {ResponseStatusCode}", response.StatusCode);
            
            return Result.Failure([EmailErrors.EmailSendFailed]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending email via Mailtrap");
            return Result.Failure([EmailErrors.EmailSendFailed]);
        }
    }
    
    public async Task<Result> SendConfirmationEmailAsync(string to, string verificationCode, CancellationToken cancellationToken = default)
    {
        var subject = "Email Confirmation - Progresium.ToDo";
        var body = BuildConfirmationEmailBody(verificationCode);
        
        return await SendEmailAsync(to, subject, body, cancellationToken);
    }

    private string BuildConfirmationEmailBody(string code)
    {
        code = HtmlEncoder.Default.Encode(code);
        
        return $@"
        <div style=""background-color: #f9fafb; padding: 50px 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #111827;"">
            <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color: #ffffff; border-radius: 8px; border: 1px solid #e5e7eb; overflow: hidden; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);"">
                <tr>
                    <td style=""padding: 40px 40px 20px 40px; text-align: center;"">
                        <h1 style=""margin: 0; color: #2563eb; font-size: 28px; font-weight: 700; letter-spacing: -0.025em;"">Progresium</h1>
                        <p style=""margin: 10px 0 0 0; color: #6b7280; font-size: 16px;"">Verify your email address</p>
                    </td>
                </tr>
                <tr>
                    <td style=""padding: 20px 40px 40px 40px;"">
                        <p style=""margin: 0 0 20px 0; font-size: 16px; line-height: 1.5;"">Hello,</p>
                        <p style=""margin: 0 0 25px 0; font-size: 16px; line-height: 1.5;"">Welcome to <strong>Progresium</strong>. To complete your account setup, please enter the following verification code in the app:</p>
                        
                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                            <tr>
                                <td align=""center"" style=""padding: 20px 0;"">
                                    <span style=""display: inline-block; background-color: #eff6ff; color: #1e40af; padding: 16px 36px; border-radius: 8px; font-size: 32px; font-weight: 700; letter-spacing: 6px; border: 1px dashed #bfdbfe;"">{code}</span>
                                </td>
                            </tr>
                        </table>

                        <p style=""margin: 20px 0 0 0; font-size: 14px; color: #6b7280; line-height: 1.5;"">This code will expire in {_mailtrapSettings.VerificationCodeLifespanInMinutes} minutes. If you did not request this code, you can safely ignore this email.</p>
                    </td>
                </tr>
                <tr>
                    <td style=""padding: 30px 40px; background-color: #f3f4f6; text-align: center; font-size: 12px; color: #9ca3af;"">
                        <p style=""margin: 0;"">&copy; 2026 Progresium. All rights reserved.</p>
                        <p style=""margin: 5px 0 0 0;"">You received this email because you signed up for Progresium.</p>
                    </td>
                </tr>
            </table>
        </div>";
    }
}
