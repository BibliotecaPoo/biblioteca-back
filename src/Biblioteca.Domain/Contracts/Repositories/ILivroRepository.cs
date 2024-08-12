using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface ILivroRepository : IRepository<Livro>
{
    void Adicionar(Livro livro);
    void Atualizar(Livro livro);
    Task<IPaginacao<Livro>> Pesquisar(int? id, string? titulo, string? autor, string? editora, string? categoria,
        int? codigo, bool? ativo, int quantidadeDeItensPorPagina = 10, int paginaAtual = 1);
    Task<List<Livro>> ObterTodos();
    IQueryable<Livro> Queryable();
}