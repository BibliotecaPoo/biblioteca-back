using Biblioteca.API.Responses;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Livro;
using Biblioteca.Application.DTOs.Paginacao;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class LivroController : BaseController
{
    private readonly ILivroService _livroService;

    public LivroController(INotificator notificator, ILivroService livroService) : base(notificator)
    {
        _livroService = livroService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar um novo livro", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Adicionar([FromBody] AdicionarLivroDto dto)
    {
        var adicionarLivro = await _livroService.Adicionar(dto);
        return CreatedResponse("", adicionarLivro);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar um livro existente", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarLivroDto dto)
    {
        var atualizarLivro = await _livroService.Atualizar(id, dto);
        return OkResponse(atualizarLivro);
    }

    [HttpPut("upload-capa/{id}")]
    [SwaggerOperation(Summary = "Adicionar/atualizar a capa de um livro", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadCapa(int id, [FromForm] ICollection<IFormFile>? files)
    {
        var adicionarCapa = await _livroService.UploadCapa(id, files);
        return CreatedResponse("", adicionarCapa);
    }

    [HttpGet("pesquisar")]
    [SwaggerOperation(Summary = "Pesquisar por livros", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(PaginacaoDto<LivroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Pesquisar([FromQuery] PesquisarLivroDto dto)
    {
        var obterLivros = await _livroService.Pesquisar(dto);
        return OkResponse(obterLivros);
    }

    [HttpGet("obter-todos")]
    [SwaggerOperation(Summary = "Obter todos os livros cadastrados", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterTodos()
    {
        var obterLivros = await _livroService.ObterTodos();
        return OkResponse(obterLivros);
    }

    [HttpPatch("ativar/{id}")]
    [SwaggerOperation(Summary = "Ativar um livro", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(int id)
    {
        await _livroService.Ativar(id);
        return NoContentResponse();
    }

    [HttpPatch("desativar/{id}")]
    [SwaggerOperation(Summary = "Desativar um livro", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(int id)
    {
        await _livroService.Desativar(id);
        return NoContentResponse();
    }
}