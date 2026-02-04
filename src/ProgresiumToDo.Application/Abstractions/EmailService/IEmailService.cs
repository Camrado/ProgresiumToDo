using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.EmailService;

public interface IEmailService
{
    Task<Result> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    
    Task<Result> SendVerificationEmailAsync(string to, string verificationCode, CancellationToken cancellationToken = default);
}