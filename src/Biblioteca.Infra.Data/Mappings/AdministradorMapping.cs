using Biblioteca.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Biblioteca.Infra.Data.Mappings;

public class AdministradorMapping : IEntityTypeConfiguration<Administrador>
{
    public void Configure(EntityTypeBuilder<Administrador> builder)
    {
        builder
            .HasKey(a => a.Id);

        builder
            .Property(a => a.Nome)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(a => a.Email)
            .IsRequired()
            .HasColumnType("VARCHAR(100)");
        
        builder
            .Property(a => a.Senha)
            .IsRequired()
            .HasColumnType("VARCHAR(255)");
        
        builder
            .Property(a => a.Ativo)
            .IsRequired()
            .HasDefaultValue(true);
        
        builder
            .Property(a => a.CodigoDeRecuperacaoDeSenha)
            .IsRequired(false);

        builder
            .Property(a => a.TempoDeExpiracaoDoCodigoDeRecuperacaoDeSenha)
            .IsRequired(false);

        builder
            .Property(a => a.PedidoDeRecuperacaoDeSenha)
            .HasDefaultValue(false);

        builder
            .Property(a => a.CriadoEm)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(a => a.AtualizadoEm)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");
    }
}