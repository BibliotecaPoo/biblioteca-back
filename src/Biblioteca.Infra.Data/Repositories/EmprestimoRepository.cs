using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infra.Data.Repositories;

public class EmprestimoRepository : Repository<Emprestimo>, IEmprestimoRepository
{
    public EmprestimoRepository(ApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Emprestimo emprestimo)
        => Context.Emprestimos.Add(emprestimo);

    public void Atualizar(Emprestimo emprestimo)
        => Context.Emprestimos.Update(emprestimo);

    public async Task<Emprestimo?> ObterPorId(int id)
        => await Context.Emprestimos.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(e => e.Id == id);

    public async Task<List<Emprestimo>> ObterTodos()
        => await Context.Emprestimos.AsNoTrackingWithIdentityResolution().ToListAsync();

    public async Task<List<Emprestimo>> ObterHistoricoDeEmprestimoDeUmUsuario(int idUsuario)
    {
        return await Context.Emprestimos
            .AsNoTrackingWithIdentityResolution()
            .Where(e => e.IdUsuario == idUsuario)
            .ToListAsync();
    }

    public async Task<List<Emprestimo>> ObterHistoricoDeEmprestimoDeUmUsuario(string matriculaUsuario)
    {
        return await Context.Emprestimos
            .AsNoTrackingWithIdentityResolution()
            .Where(e => e.Usuario.Matricula == matriculaUsuario)
            .ToListAsync();
    }

    public async Task<List<Emprestimo>> ObterHistoricoDeEmprestimoDeUmLivro(int idLivro)
    {
        return await Context.Emprestimos
            .AsNoTrackingWithIdentityResolution()
            .Where(e => e.IdLivro == idLivro)
            .ToListAsync();
    }
}