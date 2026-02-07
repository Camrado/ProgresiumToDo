using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.EmailService;

public interface IEmailService
{
    Task<Result> SendEmailAsync(List<string> tos, string subject, string body, string category, bool isHtml,
        CancellationToken cancellationToken = default);
    
    Task<Result> SendVerificationEmailAsync(string to, string verificationCode, CancellationToken cancellationToken = default);

    Task<Result> SendContactUsEmailAsync(string from, string name, string subject, string message,
        CancellationToken cancellationToken = default);
}