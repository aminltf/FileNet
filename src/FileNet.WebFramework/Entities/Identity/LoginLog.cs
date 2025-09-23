namespace FileNet.WebFramework.Entities.Identity;

public class LoginLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime AtUtc { get; set; }
    public string Ip { get; set; } = "-";
    public string? UserAgent { get; set; }
}
