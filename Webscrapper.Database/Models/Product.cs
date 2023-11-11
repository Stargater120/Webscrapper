namespace Webscrapper.Database.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Dictionary<string, string> ProductInfo { get; set; }
    public Dictionary<string, string> PriceHistory { get; set; }
    public Guid SubscriptionId { get; set; }
}