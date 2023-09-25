using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Webscrapper.CORE.Security.Authentication;

namespace Webscrapper.CORE;

public static class Startup
{
    public static IServiceCollection RegisterServices(this IServiceCollection service)
    {
        var scrapper = new FitmartScrapper();
        scrapper.Scrap();
        return service
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()))
            .AddScoped<ITokenCreator, TokenCreator>();
    }
}