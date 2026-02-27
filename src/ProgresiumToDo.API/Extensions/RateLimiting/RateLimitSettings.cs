namespace ProgresiumToDo.API.Extensions.RateLimiting;

public class RateLimitSettings
{
    public int PermitLimit { get; set; }
    public int WindowInMinutes { get; set; }
}