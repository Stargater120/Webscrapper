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
    }
    
    public IMongoCollection<FitmartItem> FitmartItems { get; set; }
    public IMongoCollection<SitePromotionBanners> PromotionBanner { get; set; }
}