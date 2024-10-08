﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Infra.Data.Migrations
{
    public partial class AdicionandoAdministradorPadrao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var senha = "$argon2id$v=19$m=32768,t=4,p=1$8kSN61J8u9f2fBanH2sbjA$mcjis6H1GOwjNVVNBznVkOkktsa+CHUc9bP95x8IsEo";
            
            migrationBuilder.InsertData(
                table: "Administradores",
                columns: new[] { "Id", "Nome", "Email", "Senha" },
                values: new object[,]
                {
                    { 1, "Administrador", "gestaobibliotecaacademica@gmail.com", senha  }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData("Administradores", "Id", 1);
        }
    }
}
