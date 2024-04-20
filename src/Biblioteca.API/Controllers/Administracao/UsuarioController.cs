using Biblioteca.API.Responses;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Usuario;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers.Administracao;

[Authorize]
[Route("v{version:apiVersion}/[controller]")]
public class UsuarioController : MainController
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(INotificator notificator, IUsuarioService usuarioService) : base(notificator)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar um usuário.", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Adicionar([FromBody] AdicionarUsuarioDto dto)
    {
        var adicionarUsuario = await _usuarioService.Adicionar(dto);
        return CreatedResponse("", adicionarUsuario);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar um usuário.", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuarioDto dto)
    {
        var atualizarUsuario = await _usuarioService.Atualizar(id, dto);
        return OkResponse(atualizarUsuario);
    }

    [HttpGet("Obter-Por-Id/{id}")]
    [SwaggerOperation(Summary = "Obter um usuário por id.", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var obterUsuario = await _usuarioService.ObterPorId(id);
        return OkResponse(obterUsuario);
    }

    [HttpGet("Obter-Por-Email/{email}")]
    [SwaggerOperation(Summary = "Obter um usuário por email.", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorEmail(string email)
    {
        var obterUsuario = await _usuarioService.ObterPorEmail(email);
        return OkResponse(obterUsuario);
    }
    
    [HttpGet("Obter-Por-Matricula/{matricula}")]
    [SwaggerOperation(Summary = "Obter um usuário por matrícula.", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorMatricula(string matricula)
    {
        var obterUsuario = await _usuarioService.ObterPorMatricula(matricula);
        return OkResponse(obterUsuario);
    }

    [HttpGet("Obter-Todos")]
    [SwaggerOperation(Summary = "Obter todos os usuários.", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObterTodos()
    {
        var obterUsuarios = await _usuarioService.ObterTodos();
        return OkResponse(obterUsuarios);
    }

    [HttpPatch("Reativar/{id}")]
    [SwaggerOperation(Summary = "Reativar um usuário.", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reativar(int id)
    {
        await _usuarioService.Reativar(id);
        return NoContent();
    }

    [HttpPatch("Desativar/{id}")]
    [SwaggerOperation(Summary = "Desativar um usuário.", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(int id)
    {
        await _usuarioService.Desativar(id);
        return NoContent();
    }
}