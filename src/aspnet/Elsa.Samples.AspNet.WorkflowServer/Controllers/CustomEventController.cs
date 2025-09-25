using Elsa.Workflows.Runtime;
using Microsoft.AspNetCore.Mvc;

namespace Elsa.Samples.AspNet.WorkflowServer.Controllers;

[ApiController]
[Route("events")]
public class CustomEventController(IEventPublisher eventPublisher) : ControllerBase
{
    [HttpPost("custom")]
    public IActionResult RunTask()
    {
        eventPublisher.PublishAsync("MyCustomEvent");
        return Ok();
    }
}