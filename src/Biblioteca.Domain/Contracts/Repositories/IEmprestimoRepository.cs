using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface IEmprestimoRepository : IRepository<Emprestimo>
{
    void Adicionar(Emprestimo emprestimo);
    void Atualizar(Emprestimo emprestimo);
    Task<Emprestimo?> ObterPorId(int id);
    Task<List<Emprestimo>> ObterTodos();
    Task<List<Emprestimo>> ObterHistoricoDeEmprestimoDeUmUsuario(int usuarioId);
    Task<List<Emprestimo>> ObterHistoricoDeEmprestimoDeUmUsuario(string usuarioMatricula);
    Task<List<Emprestimo>> ObterHistoricoDeEmprestimoDeUmLivro(int livroId);
}