using ProgresiumToDo.Application.Abstractions.Behaviors.Contracts;
using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Waitlist.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Waitlist;

namespace ProgresiumToDo.Application.Waitlist.Commands.JoinWaitlist;

internal sealed class JoinWaitlistCommandHandler : ICommandHandler<JoinWaitlistCommand, JoinWaitlistCommandResponse>
{
    private readonly IWaitlistEntryRepository _waitlistEntryRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public JoinWaitlistCommandHandler(IWaitlistEntryRepository waitlistEntryRepository,
        IEmailService emailService, IUnitOfWork unitOfWork)
    {
        _waitlistEntryRepository = waitlistEntryRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    } 
    
    public async Task<Result<JoinWaitlistCommandResponse>> Handle(JoinWaitlistCommand request, CancellationToken cancellationToken)
    {
        var emailExistsInWaitlist = await _waitlistEntryRepository.ExistsAsync(request.Email, cancellationToken);
        if (!emailExistsInWaitlist)
        {
            var waitlistEntry = WaitlistEntry.Create(request.Email);
            _waitlistEntryRepository.Add(waitlistEntry);

            await _emailService.SendWaitlistWelcomeEmailAsync(request.Email, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        return new JoinWaitlistCommandResponse("You've successfully joined the waitlist!");
    }
}