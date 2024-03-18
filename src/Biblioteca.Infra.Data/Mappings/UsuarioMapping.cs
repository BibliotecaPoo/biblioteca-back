using Biblioteca.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infra.Data.Mappings;

public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder
            .HasKey(u => u.Id);

        builder
            .Property(u => u.Nome)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasColumnType("VARCHAR(100)");

        builder
            .Property(u => u.Senha)
            .IsRequired()
            .HasColumnType("VARCHAR(255)");

        builder
            .Property(u => u.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder
            .Property(u => u.SuperUsuario)
            .IsRequired(false)
            .HasDefaultValue(false);

        builder
            .Property(u => u.CriadoEm)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(u => u.AtualizadoEm)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");
    }
}