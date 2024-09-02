using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infra.Data.Repositories;

public class AdministradorRepository : Repository<Administrador>, IAdministradorRepository
{
    public AdministradorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public void Atualizar(Administrador administrador) 
        => Context.Administradores.Update(administrador);
    public async Task<Administrador?> ObterAdministradorPorCodigoDeRecuperacaoDeSenha(string token)
    {
        return await Context.Administradores.FirstOrDefaultAsync(a => a.CodigoDeRecuperacaoDeSenha == token);
    }


}