namespace ProgresiumToDo.Application.Abstractions.EmailService;

public sealed record MailDto(
    List<string> Tos, 
    string Subject, 
    string Category, 
    string? Html, 
    string? Text);