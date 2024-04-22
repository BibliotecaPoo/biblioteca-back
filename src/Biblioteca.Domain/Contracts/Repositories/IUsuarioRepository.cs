using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    void Adicionar(Usuario usuario);
    void Atualizar(Usuario usuario);
    Task<Usuario?> ObterPorId(int id);
    Task<List<Usuario>> ObterPorEmail(string email);
    Task<List<Usuario>> ObterPorMatricula(string matricula);
    Task<List<Usuario>> ObterTodos();
}