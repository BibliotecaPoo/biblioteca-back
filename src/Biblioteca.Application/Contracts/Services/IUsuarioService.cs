using Biblioteca.Application.DTOs.Usuario;

namespace Biblioteca.Application.Contracts.Services;

public interface IUsuarioService
{
    Task<UsuarioDto?> Adicionar(AdicionarUsuarioDto dto);
    Task<UsuarioDto?> Atualizar(int id, AtualizarUsuarioDto dto);
    Task<UsuarioDto?> ObterPorId(int id);
    Task<UsuarioDto?> ObterPorEmail(string email);
    Task<List<UsuarioDto>> ObterTodos();
    Task Reativar(int id);
    Task Desativar(int id);
}