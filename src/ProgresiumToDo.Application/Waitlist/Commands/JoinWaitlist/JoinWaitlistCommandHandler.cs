using ProgresiumToDo.Application.Abstractions.BackgroundJobs;
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
    private readonly IBackgroundJobService _backgroundJobService;
    private readonly IUnitOfWork _unitOfWork;

    public JoinWaitlistCommandHandler(IWaitlistEntryRepository waitlistEntryRepository,
        IBackgroundJobService backgroundJobService, IUnitOfWork unitOfWork)
    {
        _waitlistEntryRepository = waitlistEntryRepository;
        _backgroundJobService = backgroundJobService;
        _unitOfWork = unitOfWork;
    } 
    
    public async Task<Result<JoinWaitlistCommandResponse>> Handle(JoinWaitlistCommand request, CancellationToken cancellationToken)
    {
        var emailExistsInWaitlist = await _waitlistEntryRepository.ExistsAsync(request.Email, cancellationToken);
        if (!emailExistsInWaitlist)
        {
            var waitlistEntry = WaitlistEntry.Create(request.Email);
            _waitlistEntryRepository.Add(waitlistEntry);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _backgroundJobService.EnqueueFireAndForgetJob<IEmailService>(es =>
                es.SendWaitlistWelcomeEmailAsync(request.Email, CancellationToken.None));
        }
        
        return new JoinWaitlistCommandResponse("You've successfully joined the waitlist!");
    }
}
