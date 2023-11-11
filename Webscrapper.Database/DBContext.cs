using MongoDB.Driver;
using Webscrapper.Database.Models;

namespace Webscrapper.Database;

public class DBContext
{
    private MongoClient _client;

    public DBContext(string connectionString)
    {
        _client = new MongoClient(connectionString);
        var db = _client.GetDatabase("Fitmart");
        FitmartItems = db.GetCollection<FitmartItem>("Items");
        PromotionBanner = db.GetCollection<SitePromotionBanners>("PromotionBanners");
        User = db.GetCollection<User>("Users");
        Subscription = db.GetCollection<Subscription>("Subscription");
        Websites = db.GetCollection<Website>("Website");
        Products = db.GetCollection<Product>("Product");
    }
    
    public IMongoCollection<FitmartItem> FitmartItems { get; set; }
    public IMongoCollection<SitePromotionBanners> PromotionBanner { get; set; }
    public IMongoCollection<User> User { get; set; }
    public IMongoCollection<Subscription> Subscription { get; set; }
    public IMongoCollection<Website> Websites { get; set; }
    public IMongoCollection<Product> Products { get; set; }
}