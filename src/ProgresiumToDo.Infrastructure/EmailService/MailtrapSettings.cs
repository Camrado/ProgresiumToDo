namespace ProgresiumToDo.Infrastructure.EmailService;

public sealed class MailtrapSettings
{
    public string ApiUrl { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
    public int EmailCooldownInSeconds { get; set; }
    public int VerificationCodeLifespanInMinutes { get; set; }
}