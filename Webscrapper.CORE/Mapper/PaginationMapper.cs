using Webscrapper.CORE.Payloads;
using Webscrapper.CORE.Payloads.Product;
using Webscrapper.CORE.Payloads.Subscription;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Mapper;

public interface IPaginationMapper
{
    PaginationPayload<ProductPayload> SetProductPaginatedList(IQueryable<Product> queryable, int Offset, int Limit);

    PaginationPayload<SubscriptionPayload> SetSubscriptionPaginationList(IQueryable<Subscription> queryable, int Offset,
        int Limit);
}

public class PaginationMapper : IPaginationMapper
{
    private readonly IProductMapper _productMapper;
    private readonly ISubscriptionMapper _subscriptionMapper;

    public PaginationMapper(IProductMapper productMapper, ISubscriptionMapper subscriptionMapper)
    {
        _productMapper = productMapper;
        _subscriptionMapper = subscriptionMapper;
    }

    public PaginationPayload<ProductPayload> SetProductPaginatedList(IQueryable<Product> queryable, int Offset, int Limit)
    {
        var products = queryable.Skip(Offset).Take(Limit).ToList();
        var paginated = new PaginationPayload<ProductPayload>(){Offset = Offset, Limit = Limit};
        paginated.List = _productMapper.MapProductsToProductPayloads(products);
        return paginated;
    }

    public PaginationPayload<SubscriptionPayload> SetSubscriptionPaginationList(IQueryable<Subscription> queryable, int Offset, int Limit)
    {
        var subscriptions = queryable.Skip(Offset).Take(Limit).ToList();
        var paginated = new PaginationPayload<SubscriptionPayload>(){Offset = Offset, Limit = Limit};
        paginated.List = _subscriptionMapper.MapSubscriptionToSubscriptionPayloads(subscriptions);
        return paginated; 
    }
}