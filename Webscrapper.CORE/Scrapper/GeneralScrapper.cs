using PuppeteerSharp;

namespace Webscrapper.CORE.Scrapper;

public class GeneralScrapper
{
    private IBrowser browser;

    public async Task ScrapWebSite(string url)
    {
        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
        
        
    }
}