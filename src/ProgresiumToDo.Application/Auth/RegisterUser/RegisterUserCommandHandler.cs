using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Abstractions.Onboarding;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Auth.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterUserCommandResponse>
{
    private readonly IUserOnboardingService _userOnboardingService;
    
    public RegisterUserCommandHandler(IUserOnboardingService userOnboardingService)
    {
        _userOnboardingService = userOnboardingService;
    }
    
    public async Task<Result<RegisterUserCommandResponse>> Handle(RegisterUserCommand request,
        CancellationToken cancellationToken)
    {
        var onboardingResult = await _userOnboardingService.RegisterAndOnboardUserAsync(request.Email, request.Password,
            request.FirstName, request.LastName, cancellationToken);
        
        if (onboardingResult.IsFailure)
            return Result.Failure<RegisterUserCommandResponse>(onboardingResult.Errors);
        
        return new RegisterUserCommandResponse(
            "Account created successfully.",
            onboardingResult.Value.AccessToken,
            onboardingResult.Value.RefreshToken.Token,
            onboardingResult.Value.ExpiresIn);
    }
}