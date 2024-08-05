using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface ILivroRepository : IRepository<Livro>
{
    void Adicionar(Livro livro);
    void Atualizar(Livro livro);
    void Deletar(Livro livro);
    Task<IPaginacao<Livro>> Pesquisar(int? id, string? titulo, string? autor, string? editora, string? categoria,
        int quantidadeDeItensPorPagina = 10, int paginaAtual = 1);
    Task<Livro?> ObterPorId(int id);
    Task<List<Livro>> ObterTodos();
}