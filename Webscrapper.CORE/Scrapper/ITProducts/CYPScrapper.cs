using PuppeteerSharp;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Scrapper;

public class CYPScrapper
{
    
    private IBrowser browser;

    public async Task ScrapWebSite(List<Subscription> subscriptions)
    {
        var writer = new WriteToProductsTable(new DatabaseInitializer());
        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        Dictionary<string, Dictionary<string, string>> infoDic = new();
        try
        {
            foreach (var subscription in subscriptions)
            {
                var page = await OpenSite(subscription.Url);
                infoDic = await GetData(page);
                await writer.WriteTo(infoDic, subscription);
            }
        }
        catch (Exception e)
        {

        }
        finally
        {
            await browser.CloseAsync();
        }

    }
    
    private async Task<IPage> OpenSite(string url)
    {
        IPage page = await browser.NewPageAsync();
        await page.GoToAsync(url);
        try
        {
            
            await page.WaitForSelectorAsync("#cmpwrapper", new WaitForSelectorOptions(){Timeout = 500, });
            await page.EvaluateExpressionAsync("document.querySelector('#cmpwrapper').remove()");
            await page.ScreenshotAsync(Path.Combine(Directory.GetCurrentDirectory(), ".png"));
        }
        catch (Exception e)
        {
            // ignored
        }
        
        return page;
    }

    private async Task<IPage> ChangeSite(IPage page, int pageNumber)
    {
        var pageUrl = page.Url;

        if (pageUrl.Contains("page"))
        {
            pageUrl = pageUrl.Substring(0, pageUrl.IndexOf("&page"));
        }
        
        pageUrl += $"&page={pageNumber}";

        await page.GoToAsync(pageUrl);
        try
        {
            
            await page.WaitForSelectorAsync("#cmpwrapper", new WaitForSelectorOptions(){Timeout = 500, });
            await page.EvaluateExpressionAsync("document.querySelector('#cmpwrapper').remove()");
            await page.ScreenshotAsync(Path.Combine(Directory.GetCurrentDirectory(), ".png"));
        }
        catch (Exception e)
        {
            // ignored
        }
        return page;
    }

    private async Task<Dictionary<string, Dictionary<string, string>>> GetData(IPage page)
    {
        try
        {
            await page.WaitForSelectorAsync(".tableLayout__row", new WaitForSelectorOptions(){Timeout = 1000, });
            
        }
        catch (Exception e)
        {
            // ignored
        }
        var listDives = await page.QuerySelectorAllAsync(".sc-hZFzCs");
        if (listDives.Length == 0)
        {
            await page.ScreenshotAsync(Path.Combine(Directory.GetCurrentDirectory(), ".png"));
        }
        Dictionary<string, Dictionary<string, string>> infoDic = new();
        int pageCounter = 1;
        IElementHandle emptyMessageDiv = null;
        do
        {
            var tempResult = await GetInformationFromSite(listDives);
            foreach (var result in tempResult)
            {
                if (infoDic.ContainsKey(result.Key))
                {
                    var test = "";
                    continue;
                }
                infoDic.Add(result.Key, result.Value);
            }
            Console.WriteLine($"wrote to infoDic number: {pageCounter}");
            page = await ChangeSite(page, ++pageCounter);
            await page.WaitForSelectorAsync(".tableLayout__row", new WaitForSelectorOptions(){Timeout = 2000, });
            emptyMessageDiv = await page.QuerySelectorAsync(".sc-jvLaUc");
            listDives = await page.QuerySelectorAllAsync(".sc-hZFzCs");
        } while (emptyMessageDiv is null);

        return infoDic;
    }

    private async Task<Dictionary<string, Dictionary<string, string>>> GetInformationFromSite(
        IElementHandle[] listDives)
    {
        Dictionary<string, Dictionary<string, string>> infoDic = new();




        return infoDic;
    }
    
    // sc-jvLaUc
}