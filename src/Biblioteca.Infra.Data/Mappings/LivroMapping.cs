﻿using Biblioteca.Domain.Entities;
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
            .Property(l => l.Descricao)
            .IsRequired()
            .HasColumnType("VARCHAR(200)");

        builder
            .Property(l => l.Autor)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(l => l.Editora)
            .IsRequired()
            .HasColumnType("VARCHAR(50)");

        builder
            .Property(l => l.AnoPublicacao)
            .IsRequired();

        builder
            .Property(l => l.Capa)
            .IsRequired(false)
            .HasColumnType("VARCHAR(255)");

        builder
            .Property(l => l.CriadoEm)
            .ValueGeneratedOnAdd()
            .HasColumnType("DATETIME");

        builder
            .Property(l => l.AtualizadoEm)
            .ValueGeneratedOnAddOrUpdate()
            .HasColumnType("DATETIME");
    }
}