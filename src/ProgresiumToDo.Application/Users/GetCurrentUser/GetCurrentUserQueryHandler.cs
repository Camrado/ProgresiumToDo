using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Users.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, GetCurrentUserQueryResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IIdentityService _identityService;
    
    public GetCurrentUserQueryHandler(IUserRepository userRepository, IUserContext userContext, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _userContext = userContext;
        _identityService = identityService;
    }
    
    public async Task<Result<GetCurrentUserQueryResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<GetCurrentUserQueryResponse>([UserErrors.UserNotFound]);
        }
        
        var isEmailVerified = await _identityService.IsEmailVerifiedAsync(user.Email);
        if (isEmailVerified.IsFailure)
        {
            return Result.Failure<GetCurrentUserQueryResponse>(isEmailVerified.Errors);
        }

        var userDto = new CurrentUserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            isEmailVerified.Value,
            user.CreatedAt,
            user.UpdatedAt);
        
        return new GetCurrentUserQueryResponse("User profile retrieved successfully.", userDto);
    }
}