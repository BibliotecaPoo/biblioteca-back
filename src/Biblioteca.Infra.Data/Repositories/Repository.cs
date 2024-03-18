using System.Linq.Expressions;
using Biblioteca.Domain.Contracts;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infra.Data.Repositories;

public abstract class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly ApplicationDbContext Context;
    private readonly DbSet<T> _dbSet;
    private bool _isDisposed;

    protected Repository(ApplicationDbContext context)
    {
        Context = context;
        _dbSet = context.Set<T>();
    }

    public IUnitOfWork UnitOfWork => Context;

    public async Task<T?> FirstOrDefault(Expression<Func<T, bool>> expression)
        => await _dbSet.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(expression);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
            Context.Dispose();

        _isDisposed = true;
    }

    ~Repository()
    {
        Dispose(false);
    }
}