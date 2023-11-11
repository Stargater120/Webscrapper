using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using Webscrapper.API;
using Webscrapper.CORE.Scrapper;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace ProjectVulkan.Core.HostedService;

public class RunThroughECSubscriptions : TimedHostedService
{
    public RunThroughECSubscriptions(ILogger<TimedHostedService> logger, IServiceProvider services) : base(logger, services, TimeSpan.Zero, TimeSpan.FromDays(3))
    {
    }

    protected override async Task RunJobAsync(IServiceScope serviceScope, CancellationToken stoppingToken)
    {
        var service = serviceScope.ServiceProvider.GetRequiredService<ECScrapper>();
        var appSettings =
            JsonConvert.DeserializeObject<WebscrapperAppSettings>(File.ReadAllText("appsettings.json"));
        var _context = new DBContext(appSettings.ConnectionString);

        var subscriptionFilter = Builders<Subscription>.Filter.Eq(x => x.SiteName, "Conrad");
        var subscriptions = await _context.Subscription.Find(subscriptionFilter).ToListAsync(stoppingToken);
        if (subscriptions.Count > 0)
        {
            await service.ScrapWebSite(subscriptions);
        }
    }
}