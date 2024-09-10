using Biblioteca.Domain.Entities;

namespace Biblioteca.Domain.Contracts.Repositories;

public interface IAdministradorRepository : IRepository<Administrador>
{
    void Atualizar(Administrador administrador);
    Task<Administrador?> ObterAdministradorPorCodigoDeRecuperacaoDeSenha(string token);
}