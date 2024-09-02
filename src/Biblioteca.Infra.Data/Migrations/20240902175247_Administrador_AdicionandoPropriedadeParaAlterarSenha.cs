using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Infra.Data.Migrations
{
    public partial class Administrador_AdicionandoPropriedadeParaAlterarSenha : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoDeRecuperacaoDeSenha",
                table: "Administradores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "PedidoDeRecuperacaoDeSenha",
                table: "Administradores",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TempoDeExpiracaoDoCodigoDeRecuperacaoDeSenha",
                table: "Administradores",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoDeRecuperacaoDeSenha",
                table: "Administradores");

            migrationBuilder.DropColumn(
                name: "PedidoDeRecuperacaoDeSenha",
                table: "Administradores");

            migrationBuilder.DropColumn(
                name: "TempoDeExpiracaoDoCodigoDeRecuperacaoDeSenha",
                table: "Administradores");
        }
    }
}
