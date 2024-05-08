using Biblioteca.API.Responses;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Emprestimo;
using Biblioteca.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biblioteca.API.Controllers;

[Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class EmprestimoController : BaseController
{
    private readonly IEmprestimoService _emprestimoService;

    public EmprestimoController(INotificator notificator, IEmprestimoService emprestimoService) : base(notificator)
    {
        _emprestimoService = emprestimoService;
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Realizar o empréstimo de um livro.", Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RealizarEmprestimo([FromBody] RealizarEmprestimoDto dto)
    {
        var realizarEmprestimo = await _emprestimoService.RealizarEmprestimo(dto);
        return CreatedResponse("", realizarEmprestimo);
    }

    [HttpPut("Renovacao/{id}")]
    [SwaggerOperation(Summary = "Realizar a renovação de um livro.", Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarRenovacao(int id, [FromBody] RealizarRenovacaoOuEntregaDto dto)
    {
        var realizarRenovacao = await _emprestimoService.RealizarRenovacao(id, dto);
        return OkResponse(realizarRenovacao);
    }

    [HttpPut("Entrega/{id}")]
    [SwaggerOperation(Summary = "Realizar a entrega de um livro.", Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarEntrega(int id, [FromBody] RealizarRenovacaoOuEntregaDto dto)
    {
        var realizarEntrega = await _emprestimoService.RealizarEntrega(id, dto);
        return OkResponse(realizarEntrega);
    }

    [HttpGet("Obter-Por-Id/{id}")]
    [SwaggerOperation(Summary = "Obter um empréstimo pelo id.", Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var obterEmprestimo = await _emprestimoService.ObterPorId(id);
        return OkResponse(obterEmprestimo);
    }

    [HttpGet("Obter-Todos")]
    [SwaggerOperation(Summary = "Obter todos os empréstimos.", Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterTodos()
    {
        var obterEmprestimos = await _emprestimoService.ObterTodos();
        return OkResponse(obterEmprestimos);
    }

    [HttpGet("Obter-Historico-Usuario-Por-Id/{idUsuario}")]
    [SwaggerOperation(Summary = "Obter o histórico de empréstimo de um usuário pelo id.", Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterHistoricoDeEmprestimoDeUmUsuario(int idUsuario)
    {
        var obterEmprestimos = await _emprestimoService.ObterHistoricoDeEmprestimoDeUmUsuario(idUsuario);
        return OkResponse(obterEmprestimos);
    }

    [HttpGet("Obter-Historico-Usuario-Por-Matricula/{matriculaUsuario}")]
    [SwaggerOperation(Summary = "Obter o histórico de empréstimo de um usuário pela matrícula.", Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterHistoricoDeEmprestimoDeUmUsuario(string matriculaUsuario)
    {
        var obterEmprestimos = await _emprestimoService.ObterHistoricoDeEmprestimoDeUmUsuario(matriculaUsuario);
        return OkResponse(obterEmprestimos);
    }

    [HttpGet("Obter-Historico-Livro-Por-Id/{idLivro}")]
    [SwaggerOperation(Summary = "Obter o histórico de empréstimo de um livro pelo id.", Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterHistoricoDeEmprestimoDeUmLivro(int idLivro)
    {
        var obterEmprestimos = await _emprestimoService.ObterHistoricoDeEmprestimoDeUmLivro(idLivro);
        return OkResponse(obterEmprestimos);
    }
}