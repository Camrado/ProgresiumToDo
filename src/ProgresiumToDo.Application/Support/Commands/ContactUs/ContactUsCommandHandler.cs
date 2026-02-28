using ProgresiumToDo.Application.Abstractions.BackgroundJobs;
using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Support.Commands.ContactUs;

internal sealed class ContactUsCommandHandler : ICommandHandler<ContactUsCommand, ContactUsCommandResponse>
{
    private readonly IBackgroundJobService _backgroundJobService;
    
    public ContactUsCommandHandler(IBackgroundJobService backgroundJobService)
    {
        _backgroundJobService = backgroundJobService;
    }
    
    public async Task<Result<ContactUsCommandResponse>> Handle(ContactUsCommand request, CancellationToken cancellationToken)
    {
        var contactUsForm = new ContactUsFormDto(request.Email, request.Name, request.Subject, request.Message);
        
        _backgroundJobService.EnqueueFireAndForgetJob<IEmailService>(es =>
            es.SendContactUsEmailAsync(contactUsForm, CancellationToken.None));
        
        return await Task.FromResult(
            Result.Success(new ContactUsCommandResponse("Your message has been sent successfully.")));
    }
}
