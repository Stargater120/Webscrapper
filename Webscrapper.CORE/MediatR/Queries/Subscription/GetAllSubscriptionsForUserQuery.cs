using MediatR;
using MongoDB.Driver;
using Webscrapper.CORE.Mapper;
using Webscrapper.CORE.Payloads;
using Webscrapper.CORE.Payloads.Subscription;
using Webscrapper.Database;

namespace Webscrapper.CORE.MediatR.Queries.Subscription;

public class GetAllSubscriptionsForUserQuery : IRequest<PaginationPayload<SubscriptionPayload>>
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public Guid UserId { get; set; }
}

public class GetAllSubscriptionsForUserQueryHandler : NeedsDBContext,
    IRequestHandler<GetAllSubscriptionsForUserQuery, PaginationPayload<SubscriptionPayload>>
{
    private readonly IPaginationMapper _paginationMapper;
    
    public GetAllSubscriptionsForUserQueryHandler(DatabaseInitializer databaseInitializer, IPaginationMapper paginationMapper) : base(databaseInitializer)
    {
        _paginationMapper = paginationMapper;
    }

    public async Task<PaginationPayload<SubscriptionPayload>> Handle(GetAllSubscriptionsForUserQuery request, CancellationToken cancellationToken)
    {
        var user = _dbContext.User.AsQueryable().FirstOrDefault(x => x.Id == request.UserId);
        var subscriptions = _dbContext.Subscription.AsQueryable().Where(x => user.SubscriptionIds.Contains(x.Id))
            .AsQueryable();
        var payload = _paginationMapper.SetSubscriptionPaginationList(subscriptions, request.Offset, request.Limit);

        return payload;
    }
}