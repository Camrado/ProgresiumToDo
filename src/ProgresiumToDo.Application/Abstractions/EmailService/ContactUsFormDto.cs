namespace ProgresiumToDo.Application.Abstractions.EmailService;

public sealed record ContactUsFormDto(
    string From,
    string Name, 
    string Subject, 
    string Message);