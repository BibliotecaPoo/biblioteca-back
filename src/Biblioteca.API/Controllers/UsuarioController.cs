﻿using Biblioteca.API.Responses;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Paginacao;
using Biblioteca.Application.DTOs.Usuario;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class UsuarioController : BaseController
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(INotificator notificator, IUsuarioService usuarioService) : base(notificator)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar um novo usuário", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Adicionar([FromBody] AdicionarUsuarioDto dto)
    {
        var adicionarUsuario = await _usuarioService.Adicionar(dto);
        return CreatedResponse("", adicionarUsuario);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar um usuário existente", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuarioDto dto)
    {
        var atualizarUsuario = await _usuarioService.Atualizar(id, dto);
        return OkResponse(atualizarUsuario);
    }

    [HttpGet("pesquisar")]
    [SwaggerOperation(Summary = "Pesquisar por usuários", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(typeof(PaginacaoDto<UsuarioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Pesquisar([FromQuery] PesquisarUsuarioDto dto)
    {
        var obterUsuarios = await _usuarioService.Pesquisar(dto);
        return OkResponse(obterUsuarios);
    }

    [HttpGet("obter-todos")]
    [SwaggerOperation(Summary = "Obter todos os usuários cadastrados", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(typeof(UsuarioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterTodos()
    {
        var obterUsuarios = await _usuarioService.ObterTodos();
        return OkResponse(obterUsuarios);
    }

    [HttpPatch("ativar/{id}")]
    [SwaggerOperation(Summary = "Ativar um usuário", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(int id)
    {
        await _usuarioService.Ativar(id);
        return NoContentResponse();
    }
    
    [HttpPatch("desativar/{id}")]
    [SwaggerOperation(Summary = "Desativar um usuário", Tags = new[] { "Administração - Usuários" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(int id)
    {
        await _usuarioService.Desativar(id);
        return NoContentResponse();
    }
}