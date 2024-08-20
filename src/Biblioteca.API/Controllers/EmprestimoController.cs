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
    [SwaggerOperation(Summary = "Realizar um novo empréstimo", Tags = new[] { "Administração - Empréstimos" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RealizarEmprestimo([FromBody] RealizarEmprestimoDto dto)
    {
        var realizarEmprestimo = await _emprestimoService.RealizarEmprestimo(dto);
        return CreatedResponse("", realizarEmprestimo);
    }

    [HttpPut("renovacao/{id}")]
    [SwaggerOperation(Summary = "Realizar a renovação de um empréstimo", Tags = new[] { "Administração - Empréstimos" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarRenovacao(int id, [FromBody] RealizarRenovacaoDto dto)
    {
        var realizarRenovacao = await _emprestimoService.RealizarRenovacao(id, dto);
        return OkResponse(realizarRenovacao);
    }

    [HttpPut("entrega/{id}")]
    [SwaggerOperation(Summary = "Realizar a entrega de um empréstimo", Tags = new[] { "Administração - Empréstimos" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RealizarEntrega(int id, [FromBody] RealizarEntregaDto dto)
    {
        var realizarEntrega = await _emprestimoService.RealizarEntrega(id, dto);
        return OkResponse(realizarEntrega);
    }

    [HttpGet("pesquisar")]
    [SwaggerOperation(Summary = "Pesquisar por empréstimos", Tags = new[] { "Administração - Empréstimos" })]
    [ProducesResponseType(typeof(PaginacaoDto<EmprestimoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Pesquisar([FromQuery] PesquisarEmprestimoDto dto)
    {
        var obterEmprestimos = await _emprestimoService.Pesquisar(dto);
        return OkResponse(obterEmprestimos);
    }

    [HttpGet("obter-todos")]
    [SwaggerOperation(Summary = "Obter todos os empréstimos cadastrados", Tags = new[] { "Administração - Empréstimos" })]
    [ProducesResponseType(typeof(EmprestimoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ObterTodos()
    {
        var obterEmprestimos = await _emprestimoService.ObterTodos();
        return OkResponse(obterEmprestimos);
    }
    
    [HttpPatch("ativar/{id}")]
    [SwaggerOperation(Summary = "Ativar um empréstimo", Tags = new[] { "Administração - Empréstimos" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar(int id)
    {
        await _emprestimoService.Ativar(id);
        return NoContentResponse();
    }
    
    [HttpPatch("desativar/{id}")]
    [SwaggerOperation(Summary = "Desativar um empréstimo", Tags = new[] { "Administração - Empréstimos" })]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(int id)
    {
        await _emprestimoService.Desativar(id);
        return NoContentResponse();
    }
}