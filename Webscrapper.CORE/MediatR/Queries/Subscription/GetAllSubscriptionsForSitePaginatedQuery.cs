using MediatR;
using MongoDB.Driver;
using Webscrapper.CORE.Mapper;
using Webscrapper.CORE.Payloads;
using Webscrapper.CORE.Payloads.Subscription;
using Webscrapper.Database;

namespace Webscrapper.CORE.MediatR.Queries.Subscription;

public class GetAllSubscriptionsForSitePaginatedQuery : IRequest<PaginationPayload<SubscriptionPayload>>
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public string SiteName { get; set; }
}

public class GetAllSubscriptionsForSitePaginatedQueryHandler : NeedsDBContext,
    IRequestHandler<GetAllSubscriptionsForSitePaginatedQuery, PaginationPayload<SubscriptionPayload>>
{
    private readonly IPaginationMapper _paginationMapper;
    public GetAllSubscriptionsForSitePaginatedQueryHandler(DatabaseInitializer databaseInitializer, IPaginationMapper paginationMapper) : base(databaseInitializer)
    {
        _paginationMapper = paginationMapper;
    }

    public async Task<PaginationPayload<SubscriptionPayload>> Handle(GetAllSubscriptionsForSitePaginatedQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = _dbContext.Subscription.AsQueryable().Where(x => x.SiteName == request.SiteName)
            .AsQueryable();
        var payload = _paginationMapper.SetSubscriptionPaginationList(subscriptions, request.Offset, request.Limit);

        return payload;
    }
}