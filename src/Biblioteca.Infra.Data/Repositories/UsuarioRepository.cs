using Biblioteca.Domain.Contracts;
using Biblioteca.Domain.Contracts.Repositories;
using Biblioteca.Domain.Entities;
using Biblioteca.Infra.Data.Context;
using Biblioteca.Infra.Data.Paginacao;
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

    public async Task<IPaginacao<Usuario>> Pesquisar(int? id, string? nome, string? email, string? matricula,
        string? curso, bool? ativo, int quantidadeDeItensPorPagina = 10, int paginaAtual = 1)
    {
        var consulta = Context.Usuarios
            .AsNoTracking()
            .AsQueryable();

        if (id.HasValue)
            consulta = consulta.Where(u => u.Id == id);

        if (!string.IsNullOrEmpty(nome))
            consulta = consulta.Where(u => u.Nome.Contains(nome));

        if (!string.IsNullOrEmpty(email))
            consulta = consulta.Where(u => u.Email.Contains(email));

        if (!string.IsNullOrEmpty(matricula))
            consulta = consulta.Where(u => u.Matricula.Contains(matricula));

        if (!string.IsNullOrEmpty(curso))
            consulta = consulta.Where(u => u.Curso.Contains(curso));

        if (ativo.HasValue)
            consulta = consulta.Where(u => u.Ativo == ativo);

        var resultadoPaginado = new Paginacao<Usuario>
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

    public async Task<List<Usuario>> ObterTodos()
        => await Context.Usuarios.AsNoTracking().ToListAsync();
}