using PuppeteerSharp;

namespace Webscrapper.CORE;

public class Scrapper
{
    public async Task Scrap()
    {
        var infoDic = new Dictionary<string, Dictionary<string, string>>();
        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        IBrowser browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
        
        IPage page = await browser.NewPageAsync();
        await page.GoToAsync("https://www.fitmart.de/collections/whey-protein");
        await page.WaitForSelectorAsync("#CybotCookiebotDialogBodyButtonDecline");
        await page.FocusAsync("#CybotCookiebotDialogBodyButtonDecline");
        await page.ClickAsync("#CybotCookiebotDialogBodyButtonDecline");
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
                infoDic.Add(name, entryDic);
            }
            var dir = Directory.GetCurrentDirectory();
            var path = Path.Combine(dir, "screenshot.png");
            await page.ScreenshotAsync(path);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await browser.CloseAsync();
        }
    }

    private async Task<Dictionary<string, string>> AddEntry(Dictionary<string, string> dic, string key, IJSHandle handle)
    {
        var text = await handle.JsonValueAsync<string>();
        dic.Add(key, text);
        return dic;
    }
}