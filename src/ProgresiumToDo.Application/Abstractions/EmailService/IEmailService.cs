using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.EmailService;

public interface IEmailService
{
    Task<Result> SendEmailAsync(MailDto mail, CancellationToken cancellationToken = default);
    
    Task<Result> SendVerificationEmailAsync(string to, string verificationCode, CancellationToken cancellationToken = default);

    Task<Result> SendContactUsEmailAsync(ContactUsFormDto form, CancellationToken cancellationToken = default);
}