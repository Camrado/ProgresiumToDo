using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Support.Commands.ContactUs;

internal sealed class ContactUsCommandHandler : ICommandHandler<ContactUsCommand, ContactUsCommandResponse>
{
    private readonly IEmailService _emailService;
    
    public ContactUsCommandHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }
    
    public async Task<Result<ContactUsCommandResponse>> Handle(ContactUsCommand request, CancellationToken cancellationToken)
    {
        var result = await _emailService.SendContactUsEmailAsync(request.Email, request.Name, request.Subject,
            request.Message, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<ContactUsCommandResponse>(result.Errors);
        }
        
        return Result.Success(new ContactUsCommandResponse("Your message has been sent successfully."));
    }
}