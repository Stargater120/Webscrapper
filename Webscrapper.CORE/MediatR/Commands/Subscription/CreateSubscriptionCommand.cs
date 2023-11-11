using MediatR;
using Webscrapper.Database;

namespace Webscrapper.CORE.MediatR.Commands.Subscription;

public class CreateSubscriptionCommand : IRequest<Unit>
{
    public string Url { get; set; }
    public string SiteName { get; set; }
    public string SearchTerm { get; set; }
}

public class CreateSubscriptionCommandHandler : NeedsDBContext, IRequestHandler<CreateSubscriptionCommand, Unit>
{
    public CreateSubscriptionCommandHandler(DatabaseInitializer databaseInitializer) : base(databaseInitializer)
    {
    }

    public async Task<Unit> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var subscription = new Database.Models.Subscription()
        {
            Id = Guid.NewGuid(),
            SearchTerm = request.SearchTerm,
            SiteName = request.SiteName,
            Url = request.Url
        };

        await _dbContext.Subscription.InsertOneAsync(subscription, null, cancellationToken);
        
        return Unit.Value;
    }
}