using MediatR;
using Microsoft.AspNetCore.Mvc;
using Webscrapper.CORE.MediatR.Commands;

namespace Webscrapper.API.Controllers;

[Controller, Route("Controller")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/user")]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserCommand command)
    {
        await _mediator.Send(command);

        return Ok();
    }
}