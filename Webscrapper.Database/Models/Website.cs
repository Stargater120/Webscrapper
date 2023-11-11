namespace Webscrapper.Database.Models;

public class Website
{
    public Guid Id { get; set; }
    public string SiteName { get; set; }
    public string Baselink { get; set; }
    public List<string> Categories { get; set; }
}