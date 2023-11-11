using MediatR;
using Microsoft.AspNetCore.Mvc;
using Webscrapper.CORE.MediatR.Queries.Product;

namespace Webscrapper.API.Controllers;

[Controller]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/product")]
    public async Task<IActionResult> GetProductAsync([FromQuery] GetProductQuery query)
        => Ok(await _mediator.Send(query));

    [HttpGet("/products")]
    public async Task<IActionResult> GetAllProductsPaginatedAsync([FromQuery] GetAllProductsQuery query)
        => Ok(await _mediator.Send(query));
}