using MongoDB.Driver;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Fitmart;

public class AddPromotionBannerCommand : NeedsWebscrapperContext
{
    public AddPromotionBannerCommand(DatabaseInitializer databaseInitializer) : base(databaseInitializer)
    {
    }

    public async Task AddOrUpdatePromotionBanner(PromotionBanner promotionBanner)
    {
        var filter = Builders<PromotionBanner>.Filter.Eq(x => x.SiteName, promotionBanner.SiteName);
        var banners = await _context.PromotionBanner.Find(filter).FirstAsync();
        if (String.IsNullOrWhiteSpace(banners.SiteName))
        {
            await _context.PromotionBanner.InsertOneAsync(promotionBanner);
            return;
        }

        var update = Builders<PromotionBanner>.Update.Set(x => x.Images, promotionBanner.Images)
            .Set(x => x.CreateDate, DateTime.Now);
        await _context.PromotionBanner.UpdateOneAsync(filter, update);
    }
}