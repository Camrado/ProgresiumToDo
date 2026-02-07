namespace ProgresiumToDo.Infrastructure.EmailService;

public sealed class MailtrapSettings
{
    public string ApiUrl { get; set; }
    public string NoReplyEmail { get; set; }
    public string ContactUsEmail { get; set; }
    public string SenderName { get; set; }
    public int EmailCooldownInSeconds { get; set; }
    public int VerificationCodeLifespanInMinutes { get; set; }
}