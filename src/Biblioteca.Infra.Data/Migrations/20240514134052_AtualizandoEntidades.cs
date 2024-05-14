using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Infra.Data.Migrations
{
    public partial class AtualizandoEntidades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantidadeExemplares",
                table: "Livros",
                newName: "QuantidadeExemplaresDisponiveisEmEstoque");

            migrationBuilder.AlterColumn<int>(
                name: "DiasBloqueado",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInicioBloqueio",
                table: "Usuarios",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataFimBloqueio",
                table: "Usuarios",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantidadeEmprestimosPermitida",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantidadeEmprestimosRealizados",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEmprestimo",
                table: "Emprestimos",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataDevolucaoRealizada",
                table: "Emprestimos",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataDevolucaoPrevista",
                table: "Emprestimos",
                type: "DATE",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME");

            migrationBuilder.AddColumn<int>(
                name: "QuantidadeRenovacoesPermitida",
                table: "Emprestimos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantidadeRenovacoesRealizadas",
                table: "Emprestimos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuantidadeEmprestimosPermitida",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "QuantidadeEmprestimosRealizados",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "QuantidadeRenovacoesPermitida",
                table: "Emprestimos");

            migrationBuilder.DropColumn(
                name: "QuantidadeRenovacoesRealizadas",
                table: "Emprestimos");

            migrationBuilder.RenameColumn(
                name: "QuantidadeExemplaresDisponiveisEmEstoque",
                table: "Livros",
                newName: "QuantidadeExemplares");

            migrationBuilder.AlterColumn<int>(
                name: "DiasBloqueado",
                table: "Usuarios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataInicioBloqueio",
                table: "Usuarios",
                type: "DATETIME",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataFimBloqueio",
                table: "Usuarios",
                type: "DATETIME",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEmprestimo",
                table: "Emprestimos",
                type: "DATETIME",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataDevolucaoRealizada",
                table: "Emprestimos",
                type: "DATETIME",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATE",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataDevolucaoPrevista",
                table: "Emprestimos",
                type: "DATETIME",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATE");
        }
    }
}
