using Biblioteca.Application.DTOs.Usuario;

namespace Biblioteca.Application.Contracts.Services;

public interface IUsuarioService
{
    Task<UsuarioDto?> Adicionar(AdicionarUsuarioDto dto);
    Task<UsuarioDto?> Atualizar(int id, AtualizarUsuarioDto dto);
    Task<UsuarioDto?> ObterPorId(int id);
    Task<List<UsuarioDto>> ObterPorEmail(string email);
    Task<List<UsuarioDto>> ObterPorMatricula(string matricula);
    Task<List<UsuarioDto>> ObterTodos();
}