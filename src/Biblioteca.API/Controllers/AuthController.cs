using Biblioteca.API.Responses;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Auth;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers;

[AllowAnonymous]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(INotificator notificator, IAuthService authService) : base(notificator)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Realizar login", Tags = new[] { "Autenticação" })]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.Login(dto);
        return token != null ? OkResponse(token) : Unauthorized(new[] { "Email e/ou senha incorretos." });
    }
    
    [HttpPost("esqueceu-senha")]
    [SwaggerOperation(Summary = "Esqueceu senha", Tags = new[] { "Autenticação" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EsqueceuSenha([FromQuery] string email)
    {
        await _authService.EsqueceuSenha(email);
        return OkResponse("Um e-mail foi enviado com instruções para alterar sua senha.");
    }
    
    [HttpPost("resetar-senha")]
    [SwaggerOperation(Summary = "Alterar senha", Tags = new[] { "Autenticação" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaDto dto)
    {
        await _authService.AlterarSenha(dto);
        return OkResponse("Senha alterada com sucesso.");
    }
}