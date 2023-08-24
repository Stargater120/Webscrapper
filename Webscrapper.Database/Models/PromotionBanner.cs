namespace Webscrapper.Database.Models;

public class PromotionBanner
{
    public Guid Id { get; set; }
    public DateTime CreateDate { get; set; }
    public List<byte[]> Images { get; set; } = new();
    public string SiteName { get; set; }
}