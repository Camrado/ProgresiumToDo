using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth.Errors;

namespace ProgresiumToDo.Application.Users.Commands.UpdateProfile;

internal sealed class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, UpdateProfileCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IIdentityService _identityService;
    
    public UpdateProfileCommandHandler(IUserRepository userRepository, IUserContext userContext, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _userContext = userContext;
        _identityService = identityService;
    }
    
    public async Task<Result<UpdateProfileCommandResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UpdateProfileCommandResponse>([UserErrors.UserNotFound]);
        }
        
        user.Update(request.FirstName, request.LastName);

        if (request.Email is not null)
        {
            var updateEmailResult = await _identityService.UpdateEmailAsync(user.Email, request.Email);
            if (updateEmailResult.IsFailure)
            {
                return Result.Failure<UpdateProfileCommandResponse>(updateEmailResult.Errors);
            }
            
            user.UpdateEmail(request.Email);
        }

        var updatedUserDto = new UserProfileUpdatedDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.UpdatedAt);
        
        return new UpdateProfileCommandResponse("Profile updated successfully.", updatedUserDto);
    }
}