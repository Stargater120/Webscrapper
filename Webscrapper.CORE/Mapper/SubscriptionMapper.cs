using Webscrapper.CORE.Payloads.Subscription;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Mapper;

public interface ISubscriptionMapper
{
    SubscriptionPayload MapSubscriptionToSubscriptionPayload(Subscription subscription);
    List<SubscriptionPayload> MapSubscriptionToSubscriptionPayloads(List<Subscription> subscriptions);
}

public class SubscriptionMapper : ISubscriptionMapper
{
    public SubscriptionPayload MapSubscriptionToSubscriptionPayload(Subscription subscription)
    {
        return new SubscriptionPayload()
        {
            Id = subscription.Id,
            SiteName = subscription.SiteName,
            SearchTerm = subscription.SearchTerm,
            Url = subscription.Url
        };
    }

    public List<SubscriptionPayload> MapSubscriptionToSubscriptionPayloads(List<Subscription> subscriptions)
    {
        return subscriptions.Select(MapSubscriptionToSubscriptionPayload).ToList();
    }
}