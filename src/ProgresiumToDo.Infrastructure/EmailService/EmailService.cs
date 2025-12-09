using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Infrastructure.EmailService;

internal sealed class EmailService : IEmailService
{
    public Task<Result> SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }
}