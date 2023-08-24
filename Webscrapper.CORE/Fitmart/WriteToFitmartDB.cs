using Amazon.Runtime.Internal.Transform;
using MongoDB.Driver;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Fitmart;

public class WriteToFitmartDB : NeedsWebscrapperContext
{
    public WriteToFitmartDB(DatabaseInitializer databaseInitializer) : base(databaseInitializer)
    {
    }

    public async Task CreateOrUpdateItems(Dictionary<string, Dictionary<string, string>> scrapings)
    {
        var newPriceDic = new Dictionary<string, Dictionary<string, string>>();
        var items = await _context.FitmartItems.AsQueryable().ToListAsync();
        foreach (var scraping in scrapings)
        {
            var price = scraping.Value["price"];
            price = price[1..];
            if (items.Exists(c => c.Name == scraping.Key))
            {
                
                var filter = Builders<FitmartItem>.Filter.Eq(x => x.Name, scraping.Key);
                var updater = Builders<FitmartItem>.Update.Set(x => x.Updated, DateTime.Now)
                    .Set(x => x.Price, double.Parse(price));
                var item = items.Find(c => c.Name == scraping.Key);
                if (item.Price != double.Parse(price))
                {
                    newPriceDic.Add(scraping);
                }
                await _context.FitmartItems.UpdateOneAsync(filter, updater, null,
                    CancellationToken.None);
                continue;
            }

            var entry = new FitmartItem()
            {
                CreationDate = DateTime.Now,
                Id = Guid.NewGuid(),
                Name = scraping.Key,
                Price = double.Parse(price),
                Vendor = scraping.Value["vendor"],
                Image = scraping.Value["image"]
            };
            await _context.FitmartItems.InsertOneAsync(entry);
        }
    }
}