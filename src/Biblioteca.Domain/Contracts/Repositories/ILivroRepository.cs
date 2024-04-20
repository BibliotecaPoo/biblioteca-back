using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface ILivroRepository : IRepository<Livro>
{
    void Adicionar(Livro livro);
    void Atualizar(Livro livro);
    Task<Livro?> ObterPorId(int id);
    Task<List<Livro>> ObterPorTitulo(string titulo);
    Task<List<Livro>> ObterPorAutor(string autor);
    Task<List<Livro>> ObterPorEditora(string editora);
    Task<List<Livro>> ObterTodos();
}