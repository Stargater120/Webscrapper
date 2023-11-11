using MediatR;
using MongoDB.Driver;
using Webscrapper.Database;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.MediatR.Commands.Subscription;

public class UnSubscribeCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid SubscriptionId { get; set; }
}

public class UnSubscribeCommandHandler : NeedsDBContext, IRequestHandler<UnSubscribeCommand, Unit>
{
    public UnSubscribeCommandHandler(DatabaseInitializer databaseInitializer) : base(databaseInitializer)
    {
    }

    public async Task<Unit> Handle(UnSubscribeCommand request, CancellationToken cancellationToken)
    {
        var filter = Builders<User>.Filter.Eq(x => x.Id, request.UserId);
        var user = await _dbContext.User.Find(filter).FirstAsync(cancellationToken);
        var subscriptionIndex = user.SubscriptionIds.IndexOf(request.SubscriptionId);
        if (subscriptionIndex >= 0)
        {
            user.SubscriptionIds.RemoveAt(subscriptionIndex);
        }

        var update = Builders<User>.Update.Set(x => x.SubscriptionIds, user.SubscriptionIds);
        await _dbContext.User.UpdateOneAsync(filter, update, null, cancellationToken);
        
        return Unit.Value;
    }
}