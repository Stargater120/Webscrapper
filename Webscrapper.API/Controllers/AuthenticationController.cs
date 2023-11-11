using MediatR;
using Microsoft.AspNetCore.Mvc;
using Webscrapper.CORE.MediatR.Commands;

namespace Webscrapper.API.Controllers;

[Controller]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand command)
        => Ok(await _mediator.Send(command));

    [HttpDelete("/logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}