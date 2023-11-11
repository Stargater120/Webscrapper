using PuppeteerSharp;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Scrapper;

public class ECScrapper
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
            await page.WaitForSelectorAsync("#usercentrics-root", new WaitForSelectorOptions(){Timeout = 500, });
            await page.EvaluateExpressionAsync("document.querySelector('#usercentrics-root').remove()");
            await page.ScreenshotAsync(Path.Combine(Directory.GetCurrentDirectory(), ".png"));
        }
        catch (Exception e)
        {
            // ignored
        }
        
        return page;
    }

    private async Task<IPage> ChangePage(IPage page, int number)
    {
        var pageUrl = page.Url;

        if (pageUrl.Contains("page"))
        {
            pageUrl = pageUrl.Substring(0, pageUrl.IndexOf("&page"));
        }
        
        pageUrl += $"&page={number}";

        await page.GoToAsync(pageUrl);
        try
        {
            
            await page.WaitForSelectorAsync("#usercentrics-root", new WaitForSelectorOptions(){Timeout = 2000, });
            await page.EvaluateExpressionAsync("document.querySelector('#usercentrics-root').remove()");
            await page.ScreenshotAsync(Path.Combine(Directory.GetCurrentDirectory(), ".png"));
        }
        catch (Exception e)
        {
            // ignored
        }
        return page;
    }

    private async Task<Dictionary<string, Dictionary<string, string>>> GetInformationFromSite(IElementHandle[] listDives)
    {
        var infoDic = new Dictionary<string, Dictionary<string, string>>();
        
        foreach (var listDive in listDives)
        {
            var entryDic = new Dictionary<string, string>();
            var entryDivOne = await listDive.QuerySelectorAsync(".tableLayout__column-image");
            var titleElement = await entryDivOne.QuerySelectorAsync(".product__title");
            var titleText = await (await titleElement.GetPropertyAsync("title")).JsonValueAsync<string>();
            if (infoDic.ContainsKey(titleText))
            {
                continue;
            }
            var productLink = await titleElement.GetPropertyAsync("href");
            await AddEntry(entryDic, "link", productLink);
            
            var image = await entryDivOne.QuerySelectorAsync(".product__imageLink");
            var imageSRC = await image.GetPropertyAsync("href");
            await AddEntry(entryDic, "image", imageSRC);
        
            var entryDivTwo = await listDive.QuerySelectorAsync(".tableLayout__column-contents");
            var productInfoDiv = await entryDivTwo.QuerySelectorAsync(".productDetailedInfo__inlineStack");
            var productInfoParagraphs = await productInfoDiv.QuerySelectorAllAsync("p");
            foreach (var paragraph in productInfoParagraphs)
            {
                var paragraphText = await paragraph.GetPropertyAsync("innerText");
                var productInfoText = await paragraphText.JsonValueAsync<string>();
                var splitedText = productInfoText.Split(": ");
                splitedText[0] = splitedText[0].Replace(":", "");
                entryDic.Add(splitedText[0], splitedText[1]);
            }
            
            IJSHandle sellerText;
            string sellerTextLoop = string.Empty;
            do
            {
                var seller = await entryDivTwo.QuerySelectorAsync(".product__sellerName");
                sellerText = await seller.GetPropertyAsync("innerText");
                sellerTextLoop = await sellerText.JsonValueAsync<string>();
            } while (string.IsNullOrWhiteSpace(sellerTextLoop));
            await AddEntry(entryDic, "seller", sellerText);
        
            var entryDivThree = await listDive.QuerySelectorAsync(".tableLayout__info");
            string priceText = string.Empty;
            IJSHandle price;
            do
            {
                await entryDivThree.WaitForSelectorAsync(".product__currentPrice", new WaitForSelectorOptions(){Timeout = 1000, });
                var priceElement = await entryDivThree.QuerySelectorAsync(".product__currentPrice");
                price = await priceElement.GetPropertyAsync("innerText");
                priceText = await price.JsonValueAsync<string>();
            } while (string.IsNullOrWhiteSpace(priceText));
            
            
            await AddEntry(entryDic, "price", price);
            
            var fakePriceParagraph = await entryDivThree.QuerySelectorAsync(".product__shippingInfoTax");
            if (fakePriceParagraph is null)
            {
                var ajhsgajhd = "";
                await entryDivThree.WaitForSelectorAsync(".product__shippingInfoTax", new WaitForSelectorOptions(){Timeout = 1000, });
                fakePriceParagraph = await entryDivThree.QuerySelectorAsync(".product__shippingInfoTax");
            }
            var priceInfoSpan = await fakePriceParagraph.QuerySelectorAsync("span");
            var priceInfoText = await priceInfoSpan.GetPropertyAsync("innerText");
            await AddEntry(entryDic, "priceInfo", priceInfoText);

            var shippingButton = await entryDivThree.QuerySelectorAsync(".product__shippingInfoLink");
            var shippingInfoText = await shippingButton.GetPropertyAsync("innerText");
            await AddEntry(entryDic, "shippingInfo", shippingInfoText);
            
            infoDic.Add(titleText, entryDic);
        }
        
        return infoDic;
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
        var listDives = await page.QuerySelectorAllAsync(".tableLayout__row");
        if (listDives.Length == 0)
        {
            await page.ScreenshotAsync(Path.Combine(Directory.GetCurrentDirectory(), ".png"));
        }
        Dictionary<string, Dictionary<string, string>> infoDic = new();
        int pageCounter = 1;
        try
        {
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
                page = await ChangePage(page, ++pageCounter);
                await page.WaitForSelectorAsync(".tableLayout__row", new WaitForSelectorOptions(){Timeout = 2000, });
                listDives = await page.QuerySelectorAllAsync(".tableLayout__row");
            } while (pageCounter < 5); //listDives.Length > 1
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // ignored
        }
        

        await page.ScreenshotAsync(Path.Combine(Directory.GetCurrentDirectory(), ".png"));
        return infoDic;
    }
    
    private async Task AddEntry(Dictionary<string, string> dic, string key, IJSHandle handle)
    {
        var text = await handle.JsonValueAsync<string>();
        dic.Add(key, text);
    }
}