using Biblioteca.API.Responses;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Usuario;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers.UsuarioComum;

[AllowAnonymous]
[Route("v{version:apiVersion}/[controller]")]
public class UsuarioComumController : BaseController
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioComumController(INotificator notificator, IUsuarioService usuarioService) : base(notificator)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastro.", Tags = new[] { "Usuário comum" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cadastro([FromBody] AdicionarUsuarioDto dto)
    {
        var cadastrarUsuario = await _usuarioService.Adicionar(dto);
        return CreatedResponse("", cadastrarUsuario);
    }
}