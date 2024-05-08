using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infra.Data.Repositories;

public class UsuarioRepository : Repository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Usuario usuario)
        => Context.Usuarios.Add(usuario);

    public void Atualizar(Usuario usuario)
        => Context.Usuarios.Update(usuario);

    public async Task<Usuario?> ObterPorId(int id)
        => await Context.Usuarios.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(u => u.Id == id);

    public async Task<List<Usuario>> ObterPorEmail(string email)
    {
        return await Context.Usuarios
            .AsNoTrackingWithIdentityResolution()
            .Where(u => u.Email.Contains(email))
            .ToListAsync();
    }

    public async Task<List<Usuario>> ObterPorMatricula(string matricula)
    {
        return await Context.Usuarios
            .AsNoTrackingWithIdentityResolution()
            .Where(u => u.Matricula.Contains(matricula))
            .ToListAsync();
    }

    public async Task<List<Usuario>> ObterTodos()
        => await Context.Usuarios.AsNoTrackingWithIdentityResolution().ToListAsync();
}