using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Infra.Data.Migrations
{
    public partial class UpdateLivroMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CaminhoImagemCapa",
                table: "Livros",
                newName: "Capa");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Capa",
                table: "Livros",
                newName: "CaminhoImagemCapa");
        }
    }
}
