namespace Webscrapper.CORE.Payloads.Product;

public class ProductPayload
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<string> ProductInfo { get; set; } // Key und Value vom Dictionary in einen string packen und mit einem : trennen
    public List<string> PriceHistory { get; set; }
}