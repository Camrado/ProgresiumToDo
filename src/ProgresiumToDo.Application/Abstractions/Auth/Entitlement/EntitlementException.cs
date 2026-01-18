using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.Auth.Entitlement;

public sealed class EntitlementException : Exception
{
    public List<Error> Errors { get; }
    
    public EntitlementException(List<Error> errors)
        : base("One or more entitlement errors occurred.")
    {
        Errors = errors;
    }
}