using Biblioteca.Application.DTOs.Paginacao;
using Biblioteca.Application.DTOs.Usuario;

namespace Biblioteca.Application.Contracts.Services;

public interface IUsuarioService
{
    Task<UsuarioDto?> Adicionar(AdicionarUsuarioDto dto);
    Task<UsuarioDto?> Atualizar(int id, AtualizarUsuarioDto dto);
    Task<PaginacaoDto<UsuarioDto>> Pesquisar(PesquisarUsuarioDto dto);
    Task<List<UsuarioDto>> ObterTodos();
}