using PuppeteerSharp;
using Webscrapper.CORE.Fitmart;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE;

public class BodylabScrapper
{
    private string[] sites = {"/collections/whey-protein", "/collections/pre-workout"};
    private IBrowser browser;

    public async Task ScrapeBodylab()
    {
        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
        var writer = new WriteToFitmartDB(new DatabaseInitializer());
        var bannerCommand = new AddPromotionBannerCommand(new DatabaseInitializer());
        Dictionary<string, Dictionary<string, string>> infoDic = new();
        var promotionBanner = await GetBannerImage(await OpenSite($"https://www.bodylab24.de/"));
    }
    
    private async Task<IPage> OpenSite(string url)
    {
        IPage page = await browser.NewPageAsync();
        await page.GoToAsync(url);
        try
        {
            await page.WaitForSelectorAsync("#CybotCookiebotDialogBodyButtonDecline", new WaitForSelectorOptions(){Timeout = 300, });
            await page.FocusAsync("#CybotCookiebotDialogBodyButtonDecline");
            await page.ClickAsync("#CybotCookiebotDialogBodyButtonDecline");
        }
        catch (Exception e)
        {
            // ignored
        }
        
        return page;
    }
    
    private async Task<SitePromotionBanners> GetBannerImage(IPage page)
    {
        var div = await page.QuerySelectorAsync(".flickity-slider");
        var images = await div.QuerySelectorAllAsync("img");
        var promotionBanner = new SitePromotionBanners()
        {
            CreateDate = DateTime.Now,
            Id = Guid.NewGuid(),
            SiteName = "Fitmart"
        };
        using var httpClient = new HttpClient();
        {
            foreach (var image in images)
            {
                var handle = await image.GetPropertyAsync("src");
                var src = await handle.JsonValueAsync<string>();
                var imageBytes = await httpClient.GetByteArrayAsync(src);
                promotionBanner.Images.Add(imageBytes);
            }
        }
        return promotionBanner;
    }
}