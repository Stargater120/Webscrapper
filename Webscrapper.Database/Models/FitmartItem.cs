namespace Webscrapper.Database.Models;

public class FitmartItem
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string Vendor { get; set; }
    public string Image { get; set; }
    public string State { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? Updated { get; set; }
}