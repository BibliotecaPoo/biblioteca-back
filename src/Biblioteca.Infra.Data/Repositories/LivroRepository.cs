using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Infra.Data.Repositories;

public class LivroRepository : Repository<Livro>, ILivroRepository
{
    public LivroRepository(ApplicationDbContext context) : base(context)
    {
    }

    public void Adicionar(Livro livro)
        => Context.Livros.Add(livro);

    public void Atualizar(Livro livro)
        => Context.Livros.Update(livro);

    public async Task<Livro?> ObterPorId(int id)
        => await Context.Livros.AsNoTrackingWithIdentityResolution().FirstOrDefaultAsync(l => l.Id == id);

    public async Task<List<Livro>> ObterPorTitulo(string titulo)
        => await Context.Livros.AsNoTrackingWithIdentityResolution().Where(l => l.Titulo == titulo).ToListAsync();

    public async Task<List<Livro>> ObterPorAutor(string autor)
        => await Context.Livros.AsNoTrackingWithIdentityResolution().Where(l => l.Autor == autor).ToListAsync();

    public async Task<List<Livro>> ObterPorEditora(string editora)
        => await Context.Livros.AsNoTrackingWithIdentityResolution().Where(l => l.Editora == editora).ToListAsync();

    public async Task<List<Livro>> ObterTodos()
        => await Context.Livros.AsNoTrackingWithIdentityResolution().ToListAsync();
}