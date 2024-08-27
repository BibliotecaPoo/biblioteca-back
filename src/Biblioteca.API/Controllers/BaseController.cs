using Biblioteca.API.Responses;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.API.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseController : Controller
{
    private readonly INotificator _notificator;

    protected BaseController(INotificator notificator)
    {
        _notificator = notificator;
    }

    protected IActionResult CreatedResponse(string uri = "", object? result = null)
        => CustomResponse(Created(uri, result));

    protected IActionResult OkResponse(object? result = null)
        => CustomResponse(Ok(result));

    protected IActionResult NoContentResponse()
        => CustomResponse(NoContent());

    private IActionResult CustomResponse(IActionResult objectResult)
    {
        if (OperacaoBemSucedida)
            return objectResult;

        if (_notificator.IsNotFoundResource)
            return NotFound();

        var badRequestResponse = new BadRequestResponse(_notificator.GetNotifications().ToList());
        return BadRequest(badRequestResponse);
    }

    private bool OperacaoBemSucedida => !(_notificator.HasNotification || _notificator.IsNotFoundResource);
}