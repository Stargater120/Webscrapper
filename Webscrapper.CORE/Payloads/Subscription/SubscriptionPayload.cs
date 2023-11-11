namespace Webscrapper.CORE.Payloads.Subscription;

public class SubscriptionPayload
{
    public Guid Id { get; set; }
    public string SearchTerm { get; set; }
    public string SiteName { get; set; }
    public string Url { get; set; }
}