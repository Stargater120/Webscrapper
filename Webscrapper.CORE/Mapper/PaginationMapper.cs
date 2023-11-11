using Webscrapper.CORE.Payloads;
using Webscrapper.CORE.Payloads.Product;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Mapper;

public interface IPaginationMapper
{
    PaginationPayload<ProductPayload> SetProductPaginatedList(IQueryable<Product> queryable, int Offset, int Limit);
}

public class PaginationMapper : IPaginationMapper
{
    private readonly IProductMapper _productMapper;

    public PaginationMapper(IProductMapper productMapper)
    {
        _productMapper = productMapper;
    }

    public PaginationPayload<ProductPayload> SetProductPaginatedList(IQueryable<Product> queryable, int Offset, int Limit)
    {
        var products = queryable.Skip(Offset).Take(Limit).ToList();
        var paginated = new PaginationPayload<ProductPayload>(){Offset = Offset, Limit = Limit};
        paginated.List = _productMapper.MapProductsToProductPayloads(products);
        return paginated;
    }
}