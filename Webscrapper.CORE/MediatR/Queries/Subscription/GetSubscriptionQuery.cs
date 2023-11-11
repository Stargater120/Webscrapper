using MediatR;
using MongoDB.Driver;
using Webscrapper.CORE.Mapper;
using Webscrapper.CORE.Payloads.Subscription;
using Webscrapper.Database;

namespace Webscrapper.CORE.MediatR.Queries.Subscription;

public class GetSubscriptionQuery : IRequest<SubscriptionPayload>
{
    public Guid SubscriptionId { get; set; }
}

public class GetSubscriptionQueryHandler : NeedsDBContext, IRequestHandler<GetSubscriptionQuery, SubscriptionPayload>
{
    private readonly ISubscriptionMapper _subscriptionMapper;
    
    public GetSubscriptionQueryHandler(DatabaseInitializer databaseInitializer, ISubscriptionMapper subscriptionMapper) : base(databaseInitializer)
    {
        _subscriptionMapper = subscriptionMapper;
    }

    public async Task<SubscriptionPayload> Handle(GetSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<Database.Models.Subscription>.Filter.Eq(x => x.Id, request.SubscriptionId);
        var subscription = await _dbContext.Subscription.Find(filter).FirstAsync(cancellationToken);

        return _subscriptionMapper.MapSubscriptionToSubscriptionPayload(subscription);
    }
}