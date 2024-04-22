﻿using Biblioteca.API.Responses;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Livro;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers.Administracao;

[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class LivroController : MainController
{
    private readonly ILivroService _livroService;

    public LivroController(INotificator notificator, ILivroService livroService) : base(notificator)
    {
        _livroService = livroService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Adicionar um livro.", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Adicionar([FromBody] AdicionarLivroDto dto)
    {
        var adicionarLivro = await _livroService.Adicionar(dto);
        return CreatedResponse("", adicionarLivro);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualizar um livro.", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarLivroDto dto)
    {
        var atualizarLivro = await _livroService.Atualizar(id, dto);
        return OkResponse(atualizarLivro);
    }

    [HttpPut("Upload-Capa/{id}")]
    [SwaggerOperation(Summary = "Adicionar/atualizar a capa de um livro.", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UploadCapa(int id, [FromForm] ICollection<IFormFile>? files)
    {
        var adicionarCapa = await _livroService.UploadCapa(id, files);
        return CreatedResponse("", adicionarCapa);
    }

    [HttpGet("Download-Capa/{id}")]
    [SwaggerOperation(Summary = "Baixar a capa de um livro.", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadCapa(int id)
    {
        var livro = await _livroService.ObterPorId(id);
        if (livro == null)
            return NotFound();

        if (string.IsNullOrEmpty(livro.CaminhoCapa))
            return BadRequest("Ainda não foi adicionada uma imagem de capa para essa livro.");

        var caminhoCompleto = Path.Combine("/home/guilherme/dev/imagens", livro.CaminhoCapa);
        if (!System.IO.File.Exists(caminhoCompleto))
            return NotFound();

        var conteudo = "image/jpeg";
        var nomeArquivo = livro.CaminhoCapa;

        return File(System.IO.File.OpenRead(caminhoCompleto), conteudo, nomeArquivo);
    }

    [HttpGet("Obter-Por-Id/{id}")]
    [SwaggerOperation(Summary = "Obter um livro por id.", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var obterLivro = await _livroService.ObterPorId(id);
        return OkResponse(obterLivro);
    }

    [HttpGet("Obter-Por-Titulo/{titulo}")]
    [SwaggerOperation(Summary = "Obter livros com o mesmo título.", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObterPorTitulo(string titulo)
    {
        var obterLivros = await _livroService.ObterPorTitulo(titulo);
        return OkResponse(obterLivros);
    }

    [HttpGet("Obter-Por-Autor/{autor}")]
    [SwaggerOperation(Summary = "Obter livros com o mesmo autor.", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObterPorAutor(string autor)
    {
        var obterLivros = await _livroService.ObterPorAutor(autor);
        return OkResponse(obterLivros);
    }

    [HttpGet("Obter-Por-Editora/{editora}")]
    [SwaggerOperation(Summary = "Obter livros com a mesma editora.", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObterPorEditora(string editora)
    {
        var obterLivros = await _livroService.ObterPorEditora(editora);
        return OkResponse(obterLivros);
    }

    [HttpGet("Obter-Todos")]
    [SwaggerOperation(Summary = "Obter todos os livros.", Tags = new[] { "Administração - Livros" })]
    [ProducesResponseType(typeof(LivroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ObterTodos()
    {
        var obterLivros = await _livroService.ObterTodos();
        return OkResponse(obterLivros);
    }
}