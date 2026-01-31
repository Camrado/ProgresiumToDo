using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.EmailService;

public static class EmailErrors
{
    public static Error EmailSendFailed =>new(
        "Email.SendFailed",
        "Failed to send the email due to an internal error."); 
}