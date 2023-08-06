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
    }
    
    public IMongoCollection<FitmartItem> FitmartItems { get; set; }
}