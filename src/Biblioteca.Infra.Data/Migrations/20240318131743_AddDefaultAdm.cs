using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Infra.Data.Migrations
{
    public partial class AddDefaultAdm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var senha = "$argon2id$v=19$m=32,t=4,p=1$RnR0NDk2aG41V0RMUlJaTQ$F7iAlDo/mi/fBntqGv5/uQ";
            
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Nome", "Email", "Senha", "Ativo", "SuperUsuario" },
                values: new object[,]
                {
                    { 1, "Administrador", "admin@admin.com", senha, true, true }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("Usuarios", "Id", 1);
        }
    }
}
