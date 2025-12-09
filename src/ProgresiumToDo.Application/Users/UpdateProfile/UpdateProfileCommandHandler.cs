using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Users.UpdateProfile;

internal sealed class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, UpdateProfileCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    
    public UpdateProfileCommandHandler(IUserRepository userRepository, IUserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }
    
    public async Task<Result<UpdateProfileCommandResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UpdateProfileCommandResponse>([UserErrors.UserNotFound]);
        }
        
        user.Update(request.FirstName, request.LastName);

        var updatedUserDto = new UserProfileUpdatedDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.UpdatedAt);
        
        return new UpdateProfileCommandResponse("Profile updated successfully.", updatedUserDto);
    }
}