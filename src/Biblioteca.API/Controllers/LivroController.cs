using Biblioteca.API.Responses;
using Biblioteca.Application.Configuration;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Livro;
using Biblioteca.Application.DTOs.Paginacao;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class LivroController : BaseController
{
    private readonly ILivroService _livroService;
    private readonly string _imageFolderPath;

    public LivroController(INotificator notificator, ILivroService livroService,
        IOptions<StorageSettings> storageSettings) : base(notificator)
    {
        _livroService = livroService;
        _imageFolderPath = storageSettings.Value.ImageFolderPath;
    }

    [HttpPost]
    [SwaggerOperation(Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Adicionar([FromBody] AdicionarLivroDto dto)
    {
        var adicionarLivro = await _livroService.Adicionar(dto);
        return CreatedResponse("", adicionarLivro);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarLivroDto dto)
    {
        var atualizarLivro = await _livroService.Atualizar(id, dto);
        return OkResponse(atualizarLivro);
    }

    [HttpPut("Upload-Capa/{id}")]
    [SwaggerOperation(Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadCapa(int id, [FromForm] ICollection<IFormFile>? files)
    {
        var adicionarCapa = await _livroService.UploadCapa(id, files);
        return CreatedResponse("", adicionarCapa);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(int id)
    {
        await _livroService.Deletar(id);
        return NoContentResponse();
    }

    [HttpGet("Download-Capa/{id}")]
    [SwaggerOperation(Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadCapa(int id)
    {
        var livro = await _livroService.ObterPorId(id);
        if (livro == null)
            return NotFound();

        if (string.IsNullOrEmpty(livro.NomeArquivoCapa))
            return BadRequest("Ainda não foi adicionada uma imagem de capa para essa livro.");

        var caminhoCompleto = Path.Combine(_imageFolderPath, livro.NomeArquivoCapa);
        if (!System.IO.File.Exists(caminhoCompleto))
            return NotFound();

        var conteudo = "image/jpeg";
        var nomeArquivo = livro.NomeArquivoCapa;

        return File(System.IO.File.OpenRead(caminhoCompleto), conteudo, nomeArquivo);
    }

    [HttpGet("Pesquisar")]
    [SwaggerOperation(Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(PaginacaoDto<LivroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Pesquisar([FromQuery] PesquisarLivroDto dto)
    {
        var obterLivros = await _livroService.Pesquisar(dto);
        return OkResponse(obterLivros);
    }

    [HttpGet("Obter-Todos")]
    [SwaggerOperation(Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterTodos()
    {
        var obterLivros = await _livroService.ObterTodos();
        return OkResponse(obterLivros);
    }
}