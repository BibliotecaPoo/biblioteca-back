using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface ILivroRepository : IRepository<Livro>
{
    void Adicionar(Livro livro);
    void Atualizar(Livro livro);
    Task<Livro?> ObterPorId(int id);
    Task<List<Livro>> ObterTodos();
}