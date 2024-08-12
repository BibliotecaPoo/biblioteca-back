using Biblioteca.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infra.Data.Mappings;

public class LivroMapping : IEntityTypeConfiguration<Livro>
{
    public void Configure(EntityTypeBuilder<Livro> builder)
    {
        builder
            .HasKey(l => l.Id);

        builder
            .Property(l => l.Titulo)
            .IsRequired()
            .HasColumnType("VARCHAR(100)");

        builder
            .Property(l => l.Autor)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(l => l.Edicao)
            .IsRequired()
            .HasColumnType("VARCHAR(30)");

        builder
            .Property(l => l.Editora)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(l => l.Categoria)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(l => l.Codigo)
            .IsRequired();

        builder
            .Property(l => l.AnoPublicacao)
            .IsRequired();

        builder
            .Property(l => l.QuantidadeExemplaresDisponiveisEmEstoque)
            .IsRequired();

        builder
            .Property(l => l.QuantidadeExemplaresDisponiveisParaEmprestimo)
            .IsRequired();

        builder
            .Property(l => l.StatusLivro)
            .IsRequired()
            .HasColumnType("VARCHAR(20)");

        builder
            .Property(l => l.NomeArquivoCapa)
            .IsRequired(false)
            .HasColumnType("VARCHAR(255)");

        builder
            .Property(l => l.Ativo)
            .IsRequired();

        builder
            .Property(l => l.CriadoEm)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(l => l.AtualizadoEm)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");

        builder
            .HasMany(l => l.Emprestimos)
            .WithOne(e => e.Livro);
    }
}