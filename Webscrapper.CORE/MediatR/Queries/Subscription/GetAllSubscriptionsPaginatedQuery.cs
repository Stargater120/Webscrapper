using MediatR;
using MongoDB.Driver;
using Webscrapper.CORE.Mapper;
using Webscrapper.CORE.Payloads;
using Webscrapper.CORE.Payloads.Subscription;
using Webscrapper.Database;

namespace Webscrapper.CORE.MediatR.Queries.Subscription;

public class GetAllSubscriptionsPaginatedQuery : IRequest<PaginationPayload<SubscriptionPayload>>
{
    public int Offset { get; set; }
    public int Limit { get; set; }
}

public class GetAllSubscriptionsPaginatedQueryHandler : NeedsDBContext,
    IRequestHandler<GetAllSubscriptionsPaginatedQuery, PaginationPayload<SubscriptionPayload>>
{
    private readonly IPaginationMapper _paginationMapper;
    
    public GetAllSubscriptionsPaginatedQueryHandler(DatabaseInitializer databaseInitializer, IPaginationMapper paginationMapper) : base(databaseInitializer)
    {
        _paginationMapper = paginationMapper;
    }

    public async Task<PaginationPayload<SubscriptionPayload>> Handle(GetAllSubscriptionsPaginatedQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = _dbContext.Subscription.AsQueryable().AsQueryable();
        var payload = _paginationMapper.SetSubscriptionPaginationList(subscriptions, request.Offset, request.Limit);

        return payload;
    }
}