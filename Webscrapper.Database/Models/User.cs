using System.Security.Claims;

namespace Webscrapper.Database.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<Guid> SubscriptionIds { get; set; }
    public ClaimsIdentity ClaimsIdentity { get; set; }
}