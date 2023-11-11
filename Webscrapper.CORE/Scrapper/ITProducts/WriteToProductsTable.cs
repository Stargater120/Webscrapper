using MongoDB.Driver;
using NotificationService.Types;
using Webscrapper.CORE.Fitmart;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Scrapper;

public class WriteToProductsTable : NeedsWebscrapperContext
{
    public WriteToProductsTable(DatabaseInitializer databaseInitializer) : base(databaseInitializer)
    {
    }

    public async Task WriteTo(Dictionary<string, Dictionary<string, string>> scrapings, Subscription subscription)
    {
        var newPriceDic = new Dictionary<string, Dictionary<string, string>>();
        var productFilter = Builders<Product>.Filter.Eq(x => x.SubscriptionId, subscription.Id);
        var items = await _context.Products.Find(productFilter).ToListAsync();
        var updatedItemsNotification = new EmailNotification();
        var newProducts = new List<Product>();

        try
        {
            foreach (var scraping in scrapings)
            {
                var price = scraping.Value["price"];
                if (string.IsNullOrWhiteSpace(price))
                {
                    continue;
                }
                price = price.Replace("€", "").Trim();
                double priceValue = double.Parse(price);
                if (items.Exists(c => c.Name == scraping.Key))
                {
                    var item = items.Find(x => x.Name == scraping.Key);
                    var itemPrice = item.ProductInfo["price"];
                    itemPrice = itemPrice.Replace("€", "").Trim();
                    if (double.Parse(itemPrice) != priceValue)
                    {
                        var newPriceHistory = item.PriceHistory;
                        newPriceHistory.Add(DateTime.Now.ToShortDateString(), price);
                        var update = Builders<Product>.Update.Set(x => x.PriceHistory, newPriceHistory);
                        var filter = Builders<Product>.Filter.Eq(x => x.Id, item.Id);

                        await _context.Products.UpdateOneAsync(filter, update);
                    
                        updatedItemsNotification.Items.Add(new Item()
                        {
                            Name = scraping.Key,
                            NewPrice = Double.Parse(price),
                            OldPrice = double.Parse(itemPrice)
                        });
                    }
                
                    continue;
                }
            
                newProducts.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    Name = scraping.Key,
                    PriceHistory = new Dictionary<string, string>(){{DateTime.Now.ToShortDateString(), price}},
                    ProductInfo = scraping.Value,
                    SubscriptionId = subscription.Id
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        

        await _context.Products.InsertManyAsync(newProducts);
    }
}