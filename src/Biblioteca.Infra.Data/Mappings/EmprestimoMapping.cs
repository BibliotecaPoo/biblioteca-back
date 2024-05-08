using Biblioteca.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infra.Data.Mappings;

public class EmprestimoMapping : IEntityTypeConfiguration<Emprestimo>
{
    public void Configure(EntityTypeBuilder<Emprestimo> builder)
    {
        builder
            .HasKey(e => e.Id);

        builder
            .Property(e => e.DataEmprestimo)
            .IsRequired()
            .HasColumnType("DATETIME");

        builder
            .Property(e => e.DataDevolucaoPrevista)
            .IsRequired()
            .HasColumnType("DATETIME");

        builder
            .Property(e => e.DataDevolucaoRealizada)
            .IsRequired(false)
            .HasColumnType("DATETIME");

        builder
            .Property(e => e.StatusEmprestimo)
            .IsRequired()
            .HasColumnType("VARCHAR(20)");

        builder
            .Property(e => e.IdUsuario)
            .IsRequired();

        builder
            .Property(e => e.IdLivro)
            .IsRequired();

        builder
            .Property(e => e.CriadoEm)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(e => e.AtualizadoEm)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");
    }
}