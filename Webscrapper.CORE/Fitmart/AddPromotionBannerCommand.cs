using MongoDB.Driver;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Fitmart;

public class AddPromotionBannerCommand : NeedsWebscrapperContext
{
    public AddPromotionBannerCommand(DatabaseInitializer databaseInitializer) : base(databaseInitializer)
    {
    }

    public async Task<bool> AddOrUpdatePromotionBanner(SitePromotionBanners sitePromotionBanners)
    {
        var filter = Builders<SitePromotionBanners>.Filter.Eq(x => x.SiteName, sitePromotionBanners.SiteName);
        if (await _context.PromotionBanner.EstimatedDocumentCountAsync() > 0)
        {
            var banners = await _context.PromotionBanner.Find(filter).FirstAsync();
            if (String.IsNullOrWhiteSpace(banners.SiteName))
            {
                await _context.PromotionBanner.InsertOneAsync(sitePromotionBanners);
                return false;
            }
            var update = Builders<SitePromotionBanners>.Update.Set(x => x.Images, sitePromotionBanners.Images)
                .Set(x => x.CreateDate, DateTime.Now);
            await _context.PromotionBanner.UpdateOneAsync(filter, update);
            return true;
        }
        
        await _context.PromotionBanner.InsertOneAsync(sitePromotionBanners);
        return true;
    }
}