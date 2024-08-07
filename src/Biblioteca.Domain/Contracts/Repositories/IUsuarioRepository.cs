using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    void Adicionar(Usuario usuario);
    void Atualizar(Usuario usuario);
    void Deletar(Usuario usuario);
    Task<IPaginacao<Usuario>> Pesquisar(int? id, string? nome, string? email, string? matricula, string? curso,
        int quantidadeDeItensPorPagina = 10, int paginaAtual = 1);
    Task<List<Usuario>> ObterTodos();
}