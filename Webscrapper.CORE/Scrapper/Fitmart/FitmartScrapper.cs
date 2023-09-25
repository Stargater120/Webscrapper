using NotificationService.Types;
using PuppeteerSharp;
using Webscrapper.CORE.Fitmart;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE;

public class FitmartScrapper
{
    private string[] sites = {"/collections/whey-protein", "/collections/pre-workout"};
    private IBrowser browser;
    public async Task Scrap()
    {
        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
        var writer = new WriteToFitmartDB(new DatabaseInitializer());
        var bannerCommand = new AddPromotionBannerCommand(new DatabaseInitializer());
        Dictionary<string, Dictionary<string, string>> infoDic = new();
        var promotionBanner = await GetBannerImage(await OpenSite($"https://www.fitmart.de"));
        try
        {
            foreach (var site in sites)
            {
                var page = await OpenSite($"https://www.fitmart.de{site}");
                infoDic = await GetInformationFromSite(page);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await browser.CloseAsync();
            Console.WriteLine("Scrapping done!");
        }

        var bannersGotUpdated = await bannerCommand.AddOrUpdatePromotionBanner(promotionBanner);
        var updatedEntries = await writer.CreateOrUpdateItems(infoDic);
        if (updatedEntries.Items.Count > 0 || bannersGotUpdated)
        {
            updatedEntries.Banner = promotionBanner.Images;
            var fitmartFactory = new FitmartNotificationFactory();
            fitmartFactory.CreateFitmartEMail(updatedEntries);
        }
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

    private async Task<Dictionary<string, Dictionary<string, string>>> GetInformationFromSite(IPage page)
    {
        var infoDic = new Dictionary<string, Dictionary<string, string>>();
        var dir = Directory.GetCurrentDirectory();
        var path = Path.Combine(dir, "screenshot.png");
        await page.ScreenshotAsync(path);
        var list = await page.QuerySelectorAsync(".productgrid--items");
        var listEntries = await list.QuerySelectorAllAsync(".productgrid--item");
        try
        {
            foreach (var listEntry in listEntries)
            {
                var entryDic = new Dictionary<string, string>();
                var image =
                    await (await listEntry.QuerySelectorAsync(".productitem--image-link")).QuerySelectorAsync("img");
                var imageLink = await image.GetPropertyAsync("src");
                entryDic = await AddEntry(entryDic, "image", imageLink);
                var name =
                    await (await (await listEntry.QuerySelectorAsync(".productitem--title")).GetPropertyAsync(
                        "innerText")).JsonValueAsync<string>();
                var price = await listEntry.QuerySelectorAsync(".price");
                var priceCurrent = await price.QuerySelectorAsync(".price__current");
                var money = await (await priceCurrent.QuerySelectorAsync(".money")).GetPropertyAsync("innerText");
                entryDic = await AddEntry(entryDic, "price", money);
                var vendor =
                    await (await listEntry.QuerySelectorAsync(".productitem--vendor")).GetPropertyAsync("innerText");
                entryDic = await AddEntry(entryDic, "vendor", vendor);
                var imageFigure = await (await listEntry.QuerySelectorAsync(".productitem--image-link")).QuerySelectorAsync(".productitem--image");
                var badgeSpan = await imageFigure.QuerySelectorAsync(".productitem__badge");
                var style = await badgeSpan.GetPropertyAsync("class");
                if (style.RemoteObject.Value is null)
                {
                    entryDic = await AddEntry(entryDic, "state", style);
                }
                infoDic.Add(name, entryDic);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return infoDic;
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

    private async Task<Dictionary<string, string>> AddEntry(Dictionary<string, string> dic, string key, IJSHandle handle)
    {
        var text = await handle.JsonValueAsync<string>();
        dic.Add(key, text);
        return dic;
    }
}