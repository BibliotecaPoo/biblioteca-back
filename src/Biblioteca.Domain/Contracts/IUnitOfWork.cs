namespace Biblioteca.Domain.Contracts;

public interface IUnitOfWork
{
    Task<bool> Commit();
}