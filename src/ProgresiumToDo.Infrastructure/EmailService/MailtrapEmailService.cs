using System.Net;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
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

    public async Task<Result> SendEmailAsync(MailDto mail, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailRequest = new
            {
                from = new
                {
                    email = _mailtrapSettings.NoReplyEmail,
                    name = _mailtrapSettings.SenderName
                },
                to = mail.Tos.Select(emailAddress => new { email = emailAddress }).ToArray(),
                subject = mail.Subject,
                category = mail.Category,
                html = mail.Html,
                text = mail.Text
            };

            var response = await _httpClient.PostAsJsonAsync("send", emailRequest, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            _logger.LogError("Mailtrap Error: {ResponseStatusCode}", response.StatusCode);

            return Result.Failure([EmailErrors.EmailSendFailed]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while sending email via Mailtrap");
            return Result.Failure([EmailErrors.EmailSendFailed]);
        }
    }

    public async Task<Result> SendVerificationEmailAsync(string to, string verificationCode, CancellationToken cancellationToken = default)
    {
        var htmlBody = BuildConfirmationEmailHtmlBody(verificationCode);

        var mail = new MailDto(
            [to],
            $"Email Verification: {verificationCode}",
            "Email Verification",
            htmlBody,
            null);
        
        return await SendEmailAsync(mail, cancellationToken);
    }

    public async Task<Result> SendContactUsEmailAsync(ContactUsFormDto form, CancellationToken cancellationToken = default)
    {
        var safeFrom = Sanitize(form.From);
        var safeName = Sanitize(form.Name);
        var safeSubject = Sanitize(form.Subject);
        var safeMessage = Sanitize(form.Message);
        if (string.IsNullOrWhiteSpace(safeFrom) || string.IsNullOrWhiteSpace(safeName) || string.IsNullOrWhiteSpace(safeSubject) || string.IsNullOrWhiteSpace(safeMessage))
        {
            return Result.Failure([new Error("Contact.InvalidInput", "Name, email, subject and message cannot be empty or contain only HTML.")]);
        }

        var mail = new MailDto(
            [_mailtrapSettings.ContactUsEmail],
            $"(Contact Us) - {safeSubject}",
            "Contact Us",
            null,
            $@"
From: 
{safeName} <{safeFrom}>

Message:
{safeMessage}
            "
        );
        
        return await SendEmailAsync(mail, cancellationToken);
    }
    
    public async Task<Result> SendWaitlistWelcomeEmailAsync(string to, CancellationToken cancellationToken = default)
    {
        var htmlBody = BuildWaitlistWelcomeEmailHtmlBody();

        var mail = new MailDto(
            [to],
            "Welcome to the Progresium Waitlist!",
            "Waitlist Welcome",
            htmlBody,
            null);
        
        return await SendEmailAsync(mail, cancellationToken);
    }

    private string BuildConfirmationEmailHtmlBody(string code)
    {
        const string BackgroundColor = "#f0f0f4";
        const string CardColor = "#ffffff";
        const string TextPrimary = "#15141A";
        const string TextSecondary = "#5B5B66";
        const string BrandColor = "#0060DF";

        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Verify Email</title>
    <style>
        /* Reset & Basics */
        body, table, td, a {{ -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }}
        table, td {{ mso-table-lspace: 0pt; mso-table-rspace: 0pt; }}
        img {{ -ms-interpolation-mode: bicubic; }}
        
        /* Typography */
        body {{
            font-family: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
            margin: 0;
            padding: 0;
            width: 100% !important;
            line-height: 1.5;
            color: {TextPrimary};
        }}
        
        /* Mobile Responsive */
        @media screen and (max-width: 600px) {{
            .email-container {{ width: 100% !important; }}
            .content-padding {{ padding: 20px !important; }}
            .code-block {{ font-size: 28px !important; letter-spacing: 4px !important; padding: 15px !important; }}
        }}
    </style>
</head>
<body style=""margin: 0; padding: 0; background-color: {BackgroundColor};"">
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BackgroundColor};"">
        <tr>
            <td align=""center"" style=""padding: 40px 10px;"">
                
                <table class=""email-container"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""480"" style=""background-color: {CardColor}; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); overflow: hidden;"">
                    
                    <tr>
                        <td align=""center"" style=""padding: 30px 0 0 0;"">
                            <h2 style=""margin: 0; font-size: 24px; font-weight: 800; color: {BrandColor}; letter-spacing: -0.5px;"">Progresium</h2>
                        </td>
                    </tr>

                    <tr>
                        <td class=""content-padding"" style=""padding: 30px 40px;"">
                            
                            <h1 style=""margin: 0 0 20px 0; font-size: 20px; font-weight: 700; color: {TextPrimary}; text-align: center;"">Confirm your email address</h1>
                            
                            <p style=""margin: 0 0 24px 0; font-size: 15px; line-height: 24px; color: {TextSecondary}; text-align: center;"">
                                Thanks for signing up for Progresium! Use the verification code below to complete your account setup.
                            </p>

                            <div style=""background-color: #f0f0f4; border-radius: 6px; padding: 20px; text-align: center; margin-bottom: 24px;"">
                                <span class=""code-block"" style=""font-family: 'Segoe UI'; font-size: 32px; font-weight: 700; color: {TextPrimary}; letter-spacing: 6px;"">
                                    {code}
                                </span>
                            </div>

                            <p style=""margin: 0; font-size: 13px; color: {TextSecondary}; text-align: center;"">
                                This code will expire in {_mailtrapSettings.VerificationCodeLifespanInMinutes} minutes.
                            </p>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style=""background-color: #f9f9fb; padding: 20px; text-align: center; border-top: 1px solid #eeeeee;"">
                            <p style=""margin: 0; font-size: 12px; color: #999999;"">
                                If you didn't request this code, you can safely ignore this email.
                            </p>
                        </td>
                    </tr>

                </table>
                
                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                    <tr>
                        <td align=""center"" style=""padding-top: 20px; color: #999999; font-size: 12px;"">
                            <p style=""margin: 0;"">&copy; {DateTime.Now.Year} Progresium. All rights reserved.</p> 
                        </td>
                    </tr>
                </table>

            </td>
        </tr>
    </table>

</body>
</html>";
    }
    
        private string BuildWaitlistWelcomeEmailHtmlBody()
    {
        const string BackgroundColor = "#f0f0f4";
        const string CardColor = "#ffffff";
        const string TextPrimary = "#15141A";
        const string TextSecondary = "#5B5B66";
        const string BrandColor = "#0060DF";

        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Welcome to Progresium Waitlist</title>
    <style>
        /* Reset & Basics */
        body, table, td, a {{ -webkit-text-size-adjust: 100%; -ms-text-size-adjust: 100%; }}
        table, td {{ mso-table-lspace: 0pt; mso-table-rspace: 0pt; }}
        img {{ -ms-interpolation-mode: bicubic; }}
        
        /* Typography */
        body {{
            font-family: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
            margin: 0;
            padding: 0;
            width: 100% !important;
            line-height: 1.5;
            color: {TextPrimary};
        }}
        
        /* Mobile Responsive */
        @media screen and (max-width: 600px) {{
            .email-container {{ width: 100% !important; }}
            .content-padding {{ padding: 20px !important; }}
        }}
    </style>
</head>
<body style=""margin: 0; padding: 0; background-color: {BackgroundColor};"">
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: {BackgroundColor};"">
        <tr>
            <td align=""center"" style=""padding: 40px 10px;"">
                
                <table class=""email-container"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""480"" style=""background-color: {CardColor}; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); overflow: hidden;"">
                    
                    <tr>
                        <td align=""center"" style=""padding: 30px 0 0 0;"">
                            <h2 style=""margin: 0; font-size: 24px; font-weight: 800; color: {BrandColor}; letter-spacing: -0.5px;"">Progresium</h2>
                        </td>
                    </tr>

                    <tr>
                        <td class=""content-padding"" style=""padding: 30px 40px;"">
                            
                            <h1 style=""margin: 0 0 20px 0; font-size: 20px; font-weight: 700; color: {TextPrimary}; text-align: center;"">You're on the list! 🎉</h1>
                            
                            <p style=""margin: 0 0 16px 0; font-size: 15px; line-height: 24px; color: {TextSecondary};"">
                                Hi there,
                            </p>

                            <p style=""margin: 0 0 24px 0; font-size: 15px; line-height: 24px; color: {TextSecondary};"">
                                Thank you for joining the Progresium waitlist! We're thrilled to have you on board as we prepare to launch something special.
                            </p>

                            <div style=""background-color: #f0f9ff; border-left: 4px solid {BrandColor}; border-radius: 4px; padding: 16px; margin-bottom: 24px;"">
                                <p style=""margin: 0; font-size: 14px; color: {TextPrimary}; font-weight: 600;"">What happens next?</p>
                                <p style=""margin: 8px 0 0 0; font-size: 13px; line-height: 20px; color: {TextSecondary};"">
                                    You'll be among the first to know when we launch. We'll send you updates on our progress and exclusive early access when we're ready to go live.
                                </p>
                            </div>

                            <p style=""margin: 0 0 16px 0; font-size: 15px; line-height: 24px; color: {TextSecondary};"">
                                In the meantime, feel free to spread the word and help us build an amazing community!
                            </p>

                            <p style=""margin: 0; font-size: 15px; line-height: 24px; color: {TextSecondary};"">
                                Best regards,<br>
                                <strong>The Progresium Team</strong>
                            </p>
                        </td>
                    </tr>
                    
                    <tr>
                        <td style=""background-color: #f9f9fb; padding: 20px; text-align: center; border-top: 1px solid #eeeeee;"">
                            <p style=""margin: 0; font-size: 12px; color: #999999;"">
                                You're receiving this because you joined the Progresium waitlist.
                            </p>
                        </td>
                    </tr>

                </table>
                
                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                    <tr>
                        <td align=""center"" style=""padding-top: 20px; color: #999999; font-size: 12px;"">
                            <p style=""margin: 0;"">&copy; {DateTime.Now.Year} Progresium. All rights reserved.</p> 
                        </td>
                    </tr>
                </table>

            </td>
        </tr>
    </table>

</body>
</html>";
    }
    
    private static string Sanitize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var noHtml = Regex.Replace(input, "<.*?>", string.Empty);

        var decoded = WebUtility.HtmlDecode(noHtml);

        return decoded.Trim();
    }
}
