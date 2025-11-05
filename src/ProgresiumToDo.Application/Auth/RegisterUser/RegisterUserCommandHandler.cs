using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Auth.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterUserCommandResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public RegisterUserCommandHandler(IIdentityService identityService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<RegisterUserCommandResponse>> Handle(RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var authenticationResult = await _identityService.RegisterAsync(request.Email, request.Password);

        if (authenticationResult.IsFailure)
        {
            return Result.Failure<RegisterUserCommandResponse>(authenticationResult.Errors);
        }

        var user = User.Create(request.Email, request.FirstName, request.LastName,
            authenticationResult.Value);
        
        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = _identityService.GenerateTokens(user);
        
        return new RegisterUserCommandResponse(
            "Account created successfully.",
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.ExpiresIn);
    }
}