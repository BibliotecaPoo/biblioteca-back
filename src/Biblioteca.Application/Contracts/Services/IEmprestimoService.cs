using Biblioteca.Application.DTOs.Emprestimo;
using Biblioteca.Application.DTOs.Paginacao;

namespace Biblioteca.Application.Contracts.Services;

public interface IEmprestimoService
{
    Task<EmprestimoDto?> RealizarEmprestimo(RealizarEmprestimoDto dto);
    Task<EmprestimoDto?> RealizarRenovacao(int id, RealizarRenovacaoDto dto);
    Task<EmprestimoDto?> RealizarEntrega(int id);
    Task<PaginacaoDto<EmprestimoDto>> Pesquisar(PesquisarEmprestimoDto dto);
    Task<List<EmprestimoDto>> ObterTodos();
}