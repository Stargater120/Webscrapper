using MediatR;
using Microsoft.AspNetCore.Mvc;
using Webscrapper.CORE.MediatR.Commands.Subscription;

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
}