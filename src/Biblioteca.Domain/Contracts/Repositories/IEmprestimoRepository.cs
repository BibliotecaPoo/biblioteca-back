using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface IEmprestimoRepository : IRepository<Emprestimo>
{
    void Adicionar(Emprestimo emprestimo);
    void Atualizar(Emprestimo emprestimo);
    Task<IPaginacao<Emprestimo>> Pesquisar(int? id, int? usuarioId, int? livroId, int quantidadeDeItensPorPagina = 10,
        int paginaAtual = 1);
    Task<List<Emprestimo>> ObterTodos();
}