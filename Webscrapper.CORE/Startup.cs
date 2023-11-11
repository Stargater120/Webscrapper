using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectVulkan.Core.HostedService;
using Webscrapper.CORE.Mapper;
using Webscrapper.CORE.Scrapper;
using Webscrapper.CORE.Security.Authentication;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE;

public static class Startup
{
    public static IServiceCollection RegisterServices(this IServiceCollection service)
    {
        return service
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()))
            .AddScoped<ITokenCreator, TokenCreator>()
            .AddScoped<DatabaseInitializer>()
            .AddSingleton<ECScrapper>()
            .AddScoped<IProductMapper, ProductMapper>()
            .AddScoped<IPaginationMapper, PaginationMapper>()
            .AddScoped<ISubscriptionMapper, SubscriptionMapper>()
            .AddSingleton<FitmartScrapper>();
            // .RegisterHostedServices();
    }

    private static IServiceCollection RegisterHostedServices(this IServiceCollection service)
    {
        return service
            .AddHostedService<RunThroughECSubscriptions>();
    }
}