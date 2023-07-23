using Microsoft.Extensions.DependencyInjection;

namespace Webscrapper.CORE;

public static class Startup
{
    public static IServiceCollection RegisterServices(this IServiceCollection service)
    {
        var scrapper = new Scrapper();
        scrapper.Scrap();
        return service;
    }
}