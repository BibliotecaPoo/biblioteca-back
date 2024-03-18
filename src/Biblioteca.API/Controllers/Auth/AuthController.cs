using Biblioteca.API.Responses;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Auth;
using Biblioteca.Application.DTOs.Usuario;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers.Auth;

[AllowAnonymous]
[Route("v{version:apiVersion}/[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(INotificator notificator, IAuthService authService) : base(notificator)
    {
        _authService = authService;
    }

    [HttpPost("Registrar")]
    [SwaggerOperation(Summary = "Registrar usuário.", Tags = new[] { "Usuários - Auth" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] AdicionarUsuarioDto dto)
    {
        var registrarUsuario = await _authService.Registrar(dto);
        return CreatedResponse("", registrarUsuario);
    }

    [HttpPost("Login")]
    [SwaggerOperation(Summary = "Login.", Tags = new[] { "Usuários - Auth" })]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.Login(dto);
        return token != null ? OkResponse(token) : Unauthorized(new[] { "Email e/ou senha incorretos." });
    }
}