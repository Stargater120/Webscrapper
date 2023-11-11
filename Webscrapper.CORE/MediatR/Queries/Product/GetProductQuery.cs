using MediatR;
using MongoDB.Driver;
using Webscrapper.CORE.Mapper;
using Webscrapper.CORE.Payloads.Product;
using Webscrapper.Database;

namespace Webscrapper.CORE.MediatR.Queries.Product;

public class GetProductQuery : IRequest<ProductPayload>
{
    public Guid ProductId { get; set; }
}

public class GetProductQueryHandler : NeedsDBContext, IRequestHandler<GetProductQuery, ProductPayload>
{
    private readonly IProductMapper _productMapper;
    
    public GetProductQueryHandler(DatabaseInitializer databaseInitializer, IProductMapper productMapper) : base(databaseInitializer)
    {
        _productMapper = productMapper;
    }

    public async Task<ProductPayload> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<Database.Models.Product>.Filter.Eq(x => x.Id, request.ProductId);
        var product = await _dbContext.Products.Find(filter).FirstAsync(cancellationToken);

        return _productMapper.MapProductTOProductPayload(product);
    }
}