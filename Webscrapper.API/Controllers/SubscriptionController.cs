using MediatR;
using Microsoft.AspNetCore.Mvc;
using Webscrapper.CORE.MediatR.Commands.Subscription;
using Webscrapper.CORE.MediatR.Queries.Subscription;

namespace Webscrapper.API.Controllers;

[Controller]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/subscription")]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionCommand command)
    {
        await _mediator.Send(command);

        return Ok();
    }

    [HttpGet("/subscription")]
    public async Task<IActionResult> GetSubscriptionAsync([FromQuery] GetSubscriptionQuery query)
        => Ok(await _mediator.Send(query));

    [HttpGet("/subscriptions/user")]
    public async Task<IActionResult> GetAllSubscriptionsPaginatedAsync([FromQuery] GetAllSubscriptionsForUserQuery query)
        => Ok(await _mediator.Send(query));

    [HttpGet("/subscriptions/site")]
    public async Task<IActionResult> GetAllSubscriptionsForSiteAsync([FromQuery] GetAllSubscriptionsForSitePaginatedQuery query)
        => Ok(await _mediator.Send(query));

    [HttpGet("/subscriptions")]
    public async Task<IActionResult> GetAllSubscriptionsAsync([FromQuery] GetAllSubscriptionsPaginatedQuery query)
        => Ok(await _mediator.Send(query));

    [HttpDelete("/subscription")]
    public async Task<IActionResult> UnSubscribeAsync([FromBody] UnSubscribeCommand command)
    {
        await _mediator.Send(command);

        return Ok();
    }
}