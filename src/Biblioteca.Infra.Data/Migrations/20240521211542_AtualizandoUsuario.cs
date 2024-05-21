using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Infra.Data.Migrations
{
    public partial class AtualizandoUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFimBloqueio",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DataInicioBloqueio",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "DiasBloqueado",
                table: "Usuarios");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataFimBloqueio",
                table: "Usuarios",
                type: "DATE",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicioBloqueio",
                table: "Usuarios",
                type: "DATE",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiasBloqueado",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
