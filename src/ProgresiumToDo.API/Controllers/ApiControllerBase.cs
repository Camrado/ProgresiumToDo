using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected readonly IMediator Mediator;
    
    protected ApiControllerBase(IMediator mediator)
    {
        Mediator = mediator;
    }
    
    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        var traceId = HttpContext.Features.Get<IHttpActivityFeature>()?.Activity.Id;
        var requestId = HttpContext.TraceIdentifier;
        
        var problemDetails = new ProblemDetails
        {
            Type = "https://httpstatuses.com/400",
            Title = "One or more business rules were violated.",
            Status = StatusCodes.Status400BadRequest,
            Detail = "See the errors property for more details.",
            Extensions =
            {
                ["errors"] = result.Errors,
                ["traceId"] = traceId,
                ["requestId"] = requestId
            },
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentTypes = { "application/problem+json" }
        };
    }
}