namespace Webscrapper.CORE.Payloads.Authentication;

public class LoginPayload
{
    public Guid UserId { get; set; }
    public string AuthenticationToken { get; set; }
    public string RefreshToken { get; set; }
}