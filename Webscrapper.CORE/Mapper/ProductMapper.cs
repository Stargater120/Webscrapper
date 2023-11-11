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
        var payload = new ProductPayload()
        {
            Id = product.Id,
            Name = product.Name,
            PriceHistory = new List<string>(),
            ProductInfo = new List<string>()
        };
        foreach (var infoPair in product.ProductInfo)
        {
            var combinedString = $"{infoPair.Key}: {infoPair.Value}";
            payload.ProductInfo.Add(combinedString);
        }

        foreach (var priceStep in product.PriceHistory)
        {
            var combinedString = $"{priceStep.Key}: {priceStep.Value}";
            payload.PriceHistory.Add(combinedString);
        }

        return payload;
    }

    public List<ProductPayload> MapProductsToProductPayloads(List<Product> products)
    {
        return products.Select(MapProductTOProductPayload).ToList();
    }
}