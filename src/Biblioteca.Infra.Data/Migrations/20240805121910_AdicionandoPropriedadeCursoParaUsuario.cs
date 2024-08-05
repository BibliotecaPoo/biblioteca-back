using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Infra.Data.Migrations
{
    public partial class AdicionandoPropriedadeCursoParaUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Curso",
                table: "Usuarios",
                type: "VARCHAR(100)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Curso",
                table: "Usuarios");
        }
    }
}
