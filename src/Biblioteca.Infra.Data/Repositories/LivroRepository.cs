using Biblioteca.Domain.Contracts;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data.Context;
using Biblioteca.Infra.Data.Paginacao;
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

    public async Task<IPaginacao<Livro>> Pesquisar(int? id, string? titulo, string? autor, string? editora,
        string? categoria, int? codigo, bool? ativo, int quantidadeDeItensPorPagina = 10, int paginaAtual = 1)
    {
        var consulta = Context.Livros
            .AsNoTracking()
            .AsQueryable();

        if (id.HasValue)
            consulta = consulta.Where(l => l.Id == id);

        if (!string.IsNullOrEmpty(titulo))
            consulta = consulta.Where(l => l.Titulo.Contains(titulo));

        if (!string.IsNullOrEmpty(autor))
            consulta = consulta.Where(l => l.Autor.Contains(autor));

        if (!string.IsNullOrEmpty(editora))
            consulta = consulta.Where(l => l.Editora.Contains(editora));

        if (!string.IsNullOrEmpty(categoria))
            consulta = consulta.Where(l => l.Categoria.Contains(categoria));

        if (codigo.HasValue)
            consulta = consulta.Where(l => l.Codigo == codigo);

        if (ativo.HasValue)
            consulta = consulta.Where(l => l.Ativo == ativo);

        var resultadoPaginado = new Paginacao<Livro>
        {
            TotalDeItens = await consulta.CountAsync(),
            QuantidadeDeItensPorPagina = quantidadeDeItensPorPagina,
            PaginaAtual = paginaAtual,
            Itens = await consulta.Skip((paginaAtual - 1) * quantidadeDeItensPorPagina).Take(quantidadeDeItensPorPagina)
                .ToListAsync()
        };

        var quantidadeDePaginas = (double)resultadoPaginado.TotalDeItens / quantidadeDeItensPorPagina;
        resultadoPaginado.QuantidadeDePaginas = (int)Math.Ceiling(quantidadeDePaginas);

        return resultadoPaginado;
    }

    public async Task<List<Livro>> ObterTodos()
        => await Context.Livros.AsNoTracking().ToListAsync();

    public IQueryable<Livro> Queryable()
        => Context.Livros.AsNoTracking();
}