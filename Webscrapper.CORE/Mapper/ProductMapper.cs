using Webscrapper.CORE.Payloads.Product;
using Webscrapper.Database.Models;

namespace Webscrapper.CORE.Mapper;

public interface IProductMapper
{
    public ProductPayload MapProductTOProductPayload(Product product);
    public List<ProductPayload> MapProductsToProductPayloads(List<Product> products);
}

public class ProductMapper : IProductMapper
{
    public ProductPayload MapProductTOProductPayload(Product product)
    {
        throw new NotImplementedException();
    }

    public List<ProductPayload> MapProductsToProductPayloads(List<Product> products)
    {
        throw new NotImplementedException();
    }
}