using MediatR;
using MongoDB.Driver;
using Webscrapper.CORE.Mapper;
using Webscrapper.CORE.Payloads;
using Webscrapper.CORE.Payloads.Product;
using Webscrapper.Database;

namespace Webscrapper.CORE.MediatR.Queries.Product;

public class GetAllProductsQuery : IRequest<PaginationPayload<ProductPayload>>
{
    public Guid SubscriptionId { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}

public class GetAllProductsQueryHandler : NeedsDBContext,
    IRequestHandler<GetAllProductsQuery, PaginationPayload<ProductPayload>>
{
    private readonly IPaginationMapper _paginationMapper;
    
    public GetAllProductsQueryHandler(DatabaseInitializer databaseInitializer, IPaginationMapper paginationMapper) : base(databaseInitializer)
    {
        _paginationMapper = paginationMapper;
    }

    public async Task<PaginationPayload<ProductPayload>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = _dbContext.Products.AsQueryable().Where(x => x.SubscriptionId == request.SubscriptionId)
            .AsQueryable();
        var paginatedResult = _paginationMapper.SetProductPaginatedList(products, request.Offset, request.Limit);
        return paginatedResult;
    }
}