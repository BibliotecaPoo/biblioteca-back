using Biblioteca.API.Responses;
using Biblioteca.Application.Contracts.Services;
using Biblioteca.Application.DTOs.Emprestimo;
using Biblioteca.Application.DTOs.Paginacao;
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
    [SwaggerOperation(Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RealizarEmprestimo([FromBody] RealizarEmprestimoDto dto)
    {
        var realizarEmprestimo = await _emprestimoService.RealizarEmprestimo(dto);
        return CreatedResponse("", realizarEmprestimo);
    }

    [HttpPut("Renovacao/{id}")]
    [SwaggerOperation(Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarRenovacao(int id, [FromBody] RealizarRenovacaoDto dto)
    {
        var realizarRenovacao = await _emprestimoService.RealizarRenovacao(id, dto);
        return OkResponse(realizarRenovacao);
    }

    [HttpPut("Entrega/{id}")]
    [SwaggerOperation(Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarEntrega(int id)
    {
        var realizarEntrega = await _emprestimoService.RealizarEntrega(id);
        return OkResponse(realizarEntrega);
    }

    [HttpGet("Pesquisar")]
    [SwaggerOperation(Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(PaginacaoDto<EmprestimoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Pesquisar([FromQuery] PesquisarEmprestimoDto dto)
    {
        var obterEmprestimos = await _emprestimoService.Pesquisar(dto);
        return OkResponse(obterEmprestimos);
    }

    [HttpGet("Obter-Todos")]
    [SwaggerOperation(Tags = new[] { "Administração - Empréstimo" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterTodos()
    {
        var obterEmprestimos = await _emprestimoService.ObterTodos();
        return OkResponse(obterEmprestimos);
    }
}